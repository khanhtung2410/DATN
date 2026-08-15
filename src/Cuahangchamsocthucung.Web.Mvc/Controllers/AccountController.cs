using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models;
using Abp.Zero.Configuration;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Controllers;
using Cuahangchamsocthucung.Identity;
using Cuahangchamsocthucung.MultiTenancy;
using Cuahangchamsocthucung.Sessions;
using Cuahangchamsocthucung.Web.Models.Account;
using Cuahangchamsocthucung.Web.Views.Shared.Components.TenantChange;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Web.Controllers
{
    public class AccountController : CuahangchamsocthucungControllerBase
    {
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
            INotificationPublisher notificationPublisher)
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
        }

        #region Login / Logout

        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Action(
                    "Index",
                    "Landing");
            }

            return View(new LoginFormViewModel
            {
                ReturnUrl = returnUrl,
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
                IsSelfRegistrationAllowed = IsSelfRegistrationEnabled(),
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
                null);

            await _signInManager.SignInAsync(
                loginResult.Identity,
                loginModel.RememberMe);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            var user = loginResult.User;

            var roles = await _userManager.GetRolesAsync(user);

            var targetUrl = roles.Contains(StaticRoleNames.Host.Admin)
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
            return RedirectToAction("Login");
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

        #region Register

        public ActionResult Register()
        {
            return RegisterView(new RegisterViewModel());
        }

        private ActionResult RegisterView(RegisterViewModel model)
        {
            ViewBag.IsMultiTenancyEnabled = false;
            ViewBag.IsSelfRegistrationAllowed = true;

            return View("Register", model);
        }

        private bool IsSelfRegistrationEnabled()
        {
            // Không dùng Tenant nữa
            return true;
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (model.UserName.IsNullOrEmpty() ||
                    model.Password.IsNullOrEmpty())
                {
                    throw new UserFriendlyException(
                        L("FormIsNotValidMessage"));
                }

                // Tạo User ở Host, không thuộc Tenant
                var user = new User
                {
                    TenantId = null,
                    UserName = model.UserName,
                    Name = model.Name,
                    Surname = model.Surname,
                    EmailAddress = model.EmailAddress,
                    IsActive = true,
                    IsEmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(
                    user,
                    model.Password);

                if (!createResult.Succeeded)
                {
                    throw new UserFriendlyException(
                        string.Join(
                            ", ",
                            createResult.Errors.Select(x => x.Description)
                        )
                    );
                }

                // Gán role Customer
                var roleResult = await _userManager.AddToRoleAsync(
                    user,
                    StaticRoleNames.Tenants.Customer);

                if (!roleResult.Succeeded)
                {
                    throw new UserFriendlyException(
                        string.Join(
                            ", ",
                            roleResult.Errors.Select(x => x.Description)
                        )
                    );
                }

                await _unitOfWorkManager.Current.SaveChangesAsync();

                // Đăng nhập ngay sau khi đăng ký
                var loginResult = await GetLoginResultAsync(
                    user.UserName,
                    model.Password,
                    null);

                if (loginResult.Result == AbpLoginResultType.Success)
                {
                    await _signInManager.SignInAsync(
                        loginResult.Identity,
                        false);

                    return Redirect(GetAppHomeUrl());
                }

                return RedirectToAction("Login");

            }
            catch (UserFriendlyException ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                return View("Register", model);
            }
        }

        #endregion

        #region External Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(
                "ExternalLoginCallback",
                "Account",
                new
                {
                    ReturnUrl = returnUrl
                });

            return Challenge(
                // TODO: ...?
                // new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                // {
                //     Items = { { "LoginProvider", provider } },
                //     RedirectUri = redirectUrl
                // },
                provider
            );
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null)
        {
            returnUrl = NormalizeReturnUrl(returnUrl);

            if (remoteError != null)
            {
                Logger.Error("Remote Error in ExternalLoginCallback: " + remoteError);
                throw new UserFriendlyException(L("CouldNotCompleteLoginOperation"));
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                Logger.Warn("Could not get information from external login.");
                return RedirectToAction(nameof(Login));
            }

            await _signInManager.SignOutAsync();

            var tenancyName = GetTenancyNameOrNull();

            var loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    await _signInManager.SignInAsync(loginResult.Identity, false);
                    return Redirect(returnUrl);
                case AbpLoginResultType.UnknownExternalLogin:
                    return await RegisterForExternalLogin(externalLoginInfo);
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? externalLoginInfo.ProviderKey,
                        tenancyName
                    );
            }
        }

        private async Task<ActionResult> RegisterForExternalLogin(ExternalLoginInfo externalLoginInfo)
        {
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var nameinfo = ExternalLoginInfoHelper.GetNameAndSurnameFromClaims(externalLoginInfo.Principal.Claims.ToList());

            var viewModel = new RegisterViewModel
            {
                EmailAddress = email,
                Name = nameinfo.name,
                Surname = nameinfo.surname,
                IsExternalLogin = true,
                ExternalLoginAuthSchema = externalLoginInfo.LoginProvider
            };

            if (nameinfo.name != null &&
                nameinfo.surname != null &&
                email != null)
            {
                return await Register(viewModel);
            }

            return RegisterView(viewModel);
        }

        [UnitOfWork]
        protected virtual async Task<List<Tenant>> FindPossibleTenantsOfUserAsync(UserLoginInfo login)
        {
            List<User> allUsers;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                allUsers = await _userManager.FindAllAsync(login);
            }

            return allUsers
                .Where(u => u.TenantId != null)
                .Select(u => AsyncHelper.RunSync(() => _tenantManager.FindByIdAsync(u.TenantId.Value)))
                .ToList();
        }

        #endregion

        #region 403 Forbidden
        
        [Route("/Account/Forbidden")]
        public ActionResult Error403()
        {
            return View();
        }
        
        #endregion
        
        #region Helpers

        public ActionResult RedirectToAppHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "About");
        }

        #endregion

        #region Change Tenant

        public async Task<ActionResult> TenantChangeModal()
        {
            var loginInfo = await _sessionAppService.GetCurrentLoginInformations();
            return View("/Views/Shared/Components/TenantChange/_ChangeModal.cshtml", new ChangeModalViewModel
            {
                TenancyName = loginInfo.Tenant?.TenancyName
            });
        }

        #endregion

        #region Common

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private string NormalizeReturnUrl(string returnUrl, Func<string> defaultValueBuilder = null)
        {
            if (defaultValueBuilder == null)
            {
                defaultValueBuilder = GetAppHomeUrl;
            }

            if (returnUrl.IsNullOrEmpty())
            {
                return defaultValueBuilder();
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return defaultValueBuilder();
        }

        #endregion

        #region Etc

        /// <summary>
        /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
        /// Don't use this code in production !!!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [AbpMvcAuthorize]
        public async Task<ActionResult> TestNotification(string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            var defaultTenantAdmin = new UserIdentifier(1, 2);
            var hostAdmin = new UserIdentifier(1, 1);

            await _notificationPublisher.PublishAsync(
                    "App.SimpleMessage",
                    new MessageNotificationData(message),
                    severity: NotificationSeverity.Info,
                    userIds: new[] { defaultTenantAdmin, hostAdmin }
                 );

            return Content("Sent notification: " + message);
        }

        #endregion
    }
}
