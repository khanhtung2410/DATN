using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Identity;
using Cuahangchamsocthucung.KhachHang.Dto;
using Cuahangchamsocthucung.MultiTenancy;
using Cuahangchamsocthucung.Net.Sms;
using Cuahangchamsocthucung.Sessions;
using Cuahangchamsocthucung.Web.Models.Account;
using Cuahangchamsocthucung.Web.Views.Shared.Components.TenantChange;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class AccountController : CuahangchamsocthucungControllerBase
    {
        private const int OtpLength = 6;
        private const int MaxFailedOtpAttempts = 5;
        private const int ResendCooldownSeconds = 180; // 3 minutes
        private const int MaxResendCount = 5;

        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly LogInManager _logInManager;
        private readonly SignInManager _signInManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ISessionAppService _sessionAppService;
        private readonly ITenantCache _tenantCache;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IKhachHangAppService _khachHangAppService;
        private readonly IDistributedCache _distributedCache;
        private readonly IDataProtector _dataProtector;
        private readonly ISmsSender _smsSender;

        public AccountController(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            TenantManager tenantManager,
            IUnitOfWorkManager unitOfWorkManager,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            LogInManager logInManager,
            SignInManager signInManager,
            UserRegistrationManager userRegistrationManager,
            ISessionAppService sessionAppService,
            ITenantCache tenantCache,
            INotificationPublisher notificationPublisher,
            IKhachHangAppService khachHangAppService,
            IDistributedCache distributedCache,
            IDataProtectionProvider dataProtectionProvider,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _multiTenancyConfig = multiTenancyConfig;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _logInManager = logInManager;
            _signInManager = signInManager;
            _userRegistrationManager = userRegistrationManager;
            _sessionAppService = sessionAppService;
            _tenantCache = tenantCache;
            _notificationPublisher = notificationPublisher;
            _khachHangAppService = khachHangAppService;
            _distributedCache = distributedCache;
            _dataProtector = dataProtectionProvider.CreateProtector("PendingRegistration.Protector");
            _smsSender = smsSender;
        }

        #region Login / Logout

        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Action("Index", "Landing");
            }

            return View(new LoginFormViewModel
            {
                ReturnUrl = returnUrl,
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
                MultiTenancySide = AbpSession.MultiTenancySide
            });
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<JsonResult> Login(
            LoginViewModel loginModel,
            string returnUrl = "",
            string returnUrlHash = "")
        {
            var loginResult = await GetLoginResultAsync(
                loginModel.UsernameOrEmailAddress,
                loginModel.Password,
                "Default");

            await _signInManager.SignInAsync(
                loginResult.Identity,
                loginModel.RememberMe);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            var roles = loginResult.Identity.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var isAdmin = roles.Contains(StaticRoleNames.Host.Admin)
                       || roles.Contains(StaticRoleNames.Tenants.Admin);

            var targetUrl = isAdmin
                ? Url.Action("Index", "Home")
                : Url.Action("Index", "Landing");

            return Json(new AjaxResponse
            {
                TargetUrl = targetUrl
            });
        }

        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Landing");
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        #endregion

        #region Register & OTP Flow

        public ActionResult Register()
        {
            return RegisterView(new DangKyDto());
        }

        private ActionResult RegisterView(DangKyDto model)
        {
            ViewBag.IsMultiTenancyEnabled = false;
            ViewBag.IsSelfRegistrationAllowed = true;
            return View("Register", model);
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> Register(DangKyDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Biểu mẫu không hợp lệ.";
                    return View("Register", model);
                }

                var existedUser = await _userManager.Users.AnyAsync(x => x.UserName == model.SDT);
                if (existedUser)
                {
                    ViewBag.ErrorMessage = "Số điện thoại đã được sử dụng.";
                    return View("Register", model);
                }

                // Tạo OTP 6 chữ số
                var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

                // Tạo Salt và Hash OTP
                var saltBytes = new byte[16];
                RandomNumberGenerator.Fill(saltBytes);
                var otpHash = ComputeOtpHash(otp, saltBytes);

                // Mã hóa mật khẩu trước khi lưu vào Cache
                var protectedPassword = _dataProtector.Protect(model.MatKhau);

                var pending = new
                {
                    SafeModel = new
                    {
                        HoTen = model.HoTen,
                        SDT = model.SDT,
                        Email = model.Email
                    },
                    ProtectedPassword = protectedPassword,
                    OtpHash = otpHash,
                    OtpSalt = Convert.ToBase64String(saltBytes),
                    FailedAttempts = 0,
                    ResendCount = 0,
                    LastSentAt = DateTime.UtcNow
                };

                var cacheKey = $"PendingRegistration:{model.SDT}";
                var json = JsonSerializer.Serialize(pending);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                await _distributedCache.SetStringAsync(cacheKey, json, cacheOptions);

                // Gửi SMS OTP thực tế
                await _smsSender.SendSmsAsync(model.SDT, $"Ma OTP dang ky tai khoan Cua Hang Cham Soc Thu Cung cua ban la: {otp}. Ma co hieu luc trong 5 phut.");

                return RedirectToAction(nameof(ConfirmOtp), new { sdt = model.SDT });
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.ErrorMessage = ex.Message ?? "Đã xảy ra lỗi.";
                return View("Register", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmOtp(string sdt = null)
        {
            var vm = new XacThucOtpDto { SDT = sdt };
            var remainingSeconds = 0;

            if (!string.IsNullOrWhiteSpace(sdt))
            {
                var cacheKey = $"PendingRegistration:{sdt}";
                var pendingJson = await _distributedCache.GetStringAsync(cacheKey);

                if (!string.IsNullOrWhiteSpace(pendingJson))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(pendingJson);
                        var root = doc.RootElement;
                        if (root.TryGetProperty("LastSentAt", out var lastSentAtProp) &&
                            DateTime.TryParse(lastSentAtProp.GetString(), out var lastSentAt))
                        {
                            var elapsed = DateTime.UtcNow - lastSentAt.ToUniversalTime();
                            var remain = ResendCooldownSeconds - (int)elapsed.TotalSeconds;
                            if (remain > 0) remainingSeconds = remain;
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi parse
                    }
                }
            }

            ViewBag.ResendAfterSeconds = remainingSeconds;
            return View("ConfirmOtp", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResendOtp(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt))
            {
                return Json(new { success = false, message = "Số điện thoại không hợp lệ." });
            }

            var cacheKey = $"PendingRegistration:{sdt}";
            var pendingJson = await _distributedCache.GetStringAsync(cacheKey);

            if (string.IsNullOrWhiteSpace(pendingJson))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin đăng ký hoặc mã OTP đã hết hạn. Vui lòng đăng ký lại." });
            }

            try
            {
                using var doc = JsonDocument.Parse(pendingJson);
                var root = doc.RootElement;

                DateTime? lastSentAt = null;
                var resendCount = 0;

                if (root.TryGetProperty("LastSentAt", out var lastSentAtProp) &&
                    DateTime.TryParse(lastSentAtProp.GetString(), out var parsed))
                {
                    lastSentAt = parsed.ToUniversalTime();
                }

                if (root.TryGetProperty("ResendCount", out var rcProp))
                {
                    resendCount = rcProp.GetInt32();
                }

                if (resendCount >= MaxResendCount)
                {
                    await _distributedCache.RemoveAsync(cacheKey);
                    return Json(new { success = false, message = $"Bạn đã vượt quá {MaxResendCount} lần gửi lại mã. Vui lòng đăng ký lại." });
                }

                if (lastSentAt.HasValue)
                {
                    var elapsed = DateTime.UtcNow - lastSentAt.Value;
                    if (elapsed.TotalSeconds < ResendCooldownSeconds)
                    {
                        var wait = ResendCooldownSeconds - (int)elapsed.TotalSeconds;
                        return Json(new { success = false, message = $"Vui lòng chờ {wait} giây trước khi gửi lại mã.", remainingSeconds = wait });
                    }
                }

                // Tạo OTP mới
                var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                var saltBytes = new byte[16];
                RandomNumberGenerator.Fill(saltBytes);
                var otpHash = ComputeOtpHash(otp, saltBytes);

                var safeModelElement = root.GetProperty("SafeModel");
                var safeModel = JsonSerializer.Deserialize<DangKyDto>(safeModelElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var protectedPassword = root.GetProperty("ProtectedPassword").GetString();

                var updatedPending = new
                {
                    SafeModel = new { HoTen = safeModel.HoTen, SDT = safeModel.SDT, Email = safeModel.Email },
                    ProtectedPassword = protectedPassword,
                    OtpHash = otpHash,
                    OtpSalt = Convert.ToBase64String(saltBytes),
                    FailedAttempts = 0,
                    ResendCount = resendCount + 1,
                    CreatedAt = DateTime.UtcNow,
                    LastSentAt = DateTime.UtcNow
                };

                var updatedJson = JsonSerializer.Serialize(updatedPending);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                await _distributedCache.SetStringAsync(cacheKey, updatedJson, cacheOptions);

                // Gửi lại SMS
                await _smsSender.SendSmsAsync(sdt, $"Ma OTP dang ky tai khoan moi cua ban la: {otp}. Ma co hieu luc trong 5 phut.");

                return Json(new { success = true, message = "Mã OTP đã được gửi lại.", remainingSeconds = ResendCooldownSeconds, resendCount = updatedPending.ResendCount });
            }
            catch (Exception ex)
            {
                Logger.Error("Lỗi ResendOtp", ex);
                return Json(new { success = false, message = "Không thể gửi lại mã OTP. Vui lòng thử lại sau." });
            }
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> ConfirmOtp(XacThucOtpDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Biểu mẫu không hợp lệ.";
                    return View("ConfirmOtp", input);
                }

                var cacheKey = $"PendingRegistration:{input.SDT}";
                var pendingJson = await _distributedCache.GetStringAsync(cacheKey);

                if (string.IsNullOrWhiteSpace(pendingJson))
                {
                    ViewBag.ErrorMessage = "Không tìm thấy thông tin đăng ký hoặc mã OTP đã hết hạn.";
                    return View("ConfirmOtp", input);
                }

                using var doc = JsonDocument.Parse(pendingJson);
                var root = doc.RootElement;

                var otpHash = root.GetProperty("OtpHash").GetString();
                var otpSaltB64 = root.GetProperty("OtpSalt").GetString();
                var failedAttempts = root.GetProperty("FailedAttempts").GetInt32();
                var safeModelElement = root.GetProperty("SafeModel");
                var protectedPassword = root.GetProperty("ProtectedPassword").GetString();

                var safeModel = JsonSerializer.Deserialize<DangKyDto>(safeModelElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (safeModel == null || otpHash == null || otpSaltB64 == null || protectedPassword == null)
                {
                    ViewBag.ErrorMessage = "Dữ liệu đăng ký không hợp lệ.";
                    return View("ConfirmOtp", input);
                }

                if (safeModel.SDT != input.SDT)
                {
                    ViewBag.ErrorMessage = "Số điện thoại không khớp với yêu cầu đăng ký.";
                    return View("ConfirmOtp", input);
                }

                var saltBytes = Convert.FromBase64String(otpSaltB64);
                var inputHash = ComputeOtpHash(input.Otp, saltBytes);

                if (!SecureEquals(otpHash, inputHash))
                {
                    failedAttempts++;
                    if (failedAttempts >= MaxFailedOtpAttempts)
                    {
                        await _distributedCache.RemoveAsync(cacheKey);
                        ViewBag.ErrorMessage = $"Bạn đã nhập OTP sai quá {MaxFailedOtpAttempts} lần. Vui lòng đăng ký lại.";
                        return View("ConfirmOtp", input);
                    }

                    var resendCount = root.TryGetProperty("ResendCount", out var rcProp) ? rcProp.GetInt32() : 0;
                    var updatedPending = new
                    {
                        SafeModel = new { HoTen = safeModel.HoTen, SDT = safeModel.SDT, Email = safeModel.Email },
                        ProtectedPassword = protectedPassword,
                        OtpHash = otpHash,
                        OtpSalt = otpSaltB64,
                        FailedAttempts = failedAttempts,
                        ResendCount = resendCount,
                        CreatedAt = DateTime.UtcNow,
                        LastSentAt = root.TryGetProperty("LastSentAt", out var lastSentAtProp) ? lastSentAtProp.GetString() : DateTime.UtcNow.ToString("o")
                    };

                    var updatedJson = JsonSerializer.Serialize(updatedPending);
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };
                    await _distributedCache.SetStringAsync(cacheKey, updatedJson, cacheOptions);

                    ViewBag.ErrorMessage = $"OTP không đúng. Bạn còn {MaxFailedOtpAttempts - failedAttempts} lần thử.";
                    return View("ConfirmOtp", input);
                }

                // OTP chính xác -> Giải mã mật khẩu và gọi AppService tạo tài khoản
                var plainPassword = _dataProtector.Unprotect(protectedPassword);

                var registrationDto = new DangKyDto
                {
                    HoTen = safeModel.HoTen,
                    SDT = safeModel.SDT,
                    Email = safeModel.Email,
                    MatKhau = plainPassword,
                    XacNhanMatKhau = plainPassword
                };

                await _khachHangAppService.DangKy(registrationDto);

                // Xóa Cache đăng ký tạm
                await _distributedCache.RemoveAsync(cacheKey);

                TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.ErrorMessage = ex.Message ?? "Đã xảy ra lỗi.";
                return View("ConfirmOtp", input);
            }
        }

        #endregion

        #region Helpers

        private static string ComputeOtpHash(string otp, byte[] salt)
        {
            using var hmac = new HMACSHA256(salt);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(hashBytes);
        }

        private static bool SecureEquals(string a, string b)
        {
            if (a == null || b == null) return false;
            var aBytes = Convert.FromBase64String(a);
            var bBytes = Convert.FromBase64String(b);
            if (aBytes.Length != bBytes.Length) return false;
            var diff = 0;
            for (var i = 0; i < aBytes.Length; i++)
            {
                diff |= aBytes[i] ^ bBytes[i];
            }
            return diff == 0;
        }

        #endregion
    }
}