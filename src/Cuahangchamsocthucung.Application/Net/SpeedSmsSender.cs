using Abp;
using Abp.Dependency;
using Abp.Logging;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Net.Sms
{
    public class SpeedSmsSender : ISmsSender, ITransientDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SpeedSmsSender(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            Debug.WriteLine("========== SPEEDSMS 20/08/2026 ==========");
            Debug.WriteLine($"Phone ban đầu: {phoneNumber}");

            var apiUrl = _configuration["SmsSettings:ApiUrl"];
            var accessToken = _configuration["SmsSettings:AccessToken"];
            var deviceId = _configuration["SmsSettings:DeviceID"];

            Debug.WriteLine($"SpeedSMS URL: {apiUrl}");
            Debug.WriteLine($"AccessToken: {(string.IsNullOrWhiteSpace(accessToken) ? "KHÔNG CÓ" : "ĐÃ CÓ")}");

            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new UserFriendlyException("Chưa cấu hình SmsSettings:ApiUrl.");
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new UserFriendlyException("Chưa cấu hình SmsSettings:AccessToken.");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new UserFriendlyException("Số điện thoại không hợp lệ.");
            if (string.IsNullOrWhiteSpace(message))
                throw new UserFriendlyException("Nội dung SMS không được để trống.");

            var formattedPhone = FormatPhoneNumber(phoneNumber);
            Debug.WriteLine($"Phone sau khi format: {formattedPhone}");

            var client = _httpClientFactory.CreateClient();
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accessToken}:x"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

            var payload = new
            {
                to = new[] { formattedPhone },
                content = message,
                sms_type = 5,
                sender= deviceId
            };

            var json = JsonSerializer.Serialize(payload);

            Debug.WriteLine($"SpeedSMS Payload: {json}");
            Debug.WriteLine("ĐANG GỬI REQUEST...");

            using var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync(apiUrl, httpContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("========== SPEEDSMS CONNECTION ERROR ==========");
                Debug.WriteLine(ex.ToString());
                LogHelper.Logger.Error("Không thể kết nối đến SpeedSMS.", ex);
                throw new UserFriendlyException("Không thể kết nối đến hệ thống gửi SMS.");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("========== SPEEDSMS RESPONSE ==========");
            Debug.WriteLine($"HTTP Status: {(int)response.StatusCode}");
            Debug.WriteLine($"Response Body: {responseBody}");
            Debug.WriteLine("========================================");

            LogHelper.Logger.Info($"SpeedSMS Response: HTTP={(int)response.StatusCode}, Body={responseBody}");

            if (!response.IsSuccessStatusCode)
                throw new UserFriendlyException($"SpeedSMS trả về HTTP {(int)response.StatusCode}: {responseBody}");

            try
            {
                using var document = JsonDocument.Parse(responseBody);
                var root = document.RootElement;

                var status = root.TryGetProperty("status", out var statusProperty)
                    ? statusProperty.ToString()
                    : "";

                var code = root.TryGetProperty("code", out var codeProperty)
                    ? codeProperty.ToString()
                    : "N/A";

                var errorMessage = root.TryGetProperty("message", out var messageProperty)
                    ? messageProperty.ToString()
                    : "";

                Debug.WriteLine($"SpeedSMS Status: {status}");
                Debug.WriteLine($"SpeedSMS Code: {code}");
                Debug.WriteLine($"SpeedSMS Message: {errorMessage}");

                if (!string.Equals(status, "success", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("========== GỬI SMS THẤT BẠI ==========");

                    throw new UserFriendlyException(
                        $"Gửi SMS thất bại. Mã lỗi: {code}. {errorMessage}");
                }

                Debug.WriteLine("========== GỬI SMS THÀNH CÔNG ==========");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine("========== JSON ERROR ==========");
                Debug.WriteLine(ex.ToString());
                LogHelper.Logger.Error("SpeedSMS trả về JSON không hợp lệ.", ex);
                throw new UserFriendlyException($"SpeedSMS trả về dữ liệu không hợp lệ: {responseBody}");
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            phone = phone.Trim();

            if (phone.StartsWith("+84"))
                return phone.Substring(1);

            if (phone.StartsWith("84"))
                return phone;

            if (phone.StartsWith("0"))
                return "84" + phone.Substring(1);

            return phone;
        }
    }
}