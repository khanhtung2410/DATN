using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Microsoft.Extensions.Configuration;

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
            // 1. Kiểm tra thông tin cấu hình từ appsettings.json
            var apiUrl = _configuration["SmsSettings:ApiUrl"] ?? "https://api.speedsms.vn/index.php/sms/send";
            var apiKey = _configuration["SmsSettings:AccessToken"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new UserFriendlyException("Chưa cấu hình AccessToken trong file appsettings.json!");
            }

            var client = _httpClientFactory.CreateClient();

            // 2. Authentication: Basic Auth [Base64(AccessToken:x)]
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:x"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            // 3. Chuẩn hóa Payload theo đúng tài liệu SpeedSMS API
            var payload = new
            {
                to = new[] { FormatPhoneNumber(phoneNumber) },
                content = message,
                sms_type = 4, // 4 = Tin nhắn OTP/Thông báo từ đầu số ngẫu nhiên (Không cần Brandname)
                sender = ""   // Đúng tên thuộc tính API quy định (thay vì brandname)
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            // 4. Bắt buộc đọc body JSON vì SpeedSMS luôn trả về HTTP StatusCode = 200 OK
            var responseContent = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("status", out var statusProp))
            {
                var status = statusProp.GetString();
                if (status != "success")
                {
                    // Lấy mã lỗi và thông điệp lỗi từ SpeedSMS để hiển thị rõ ràng
                    var errCode = root.TryGetProperty("code", out var codeProp) ? codeProp.ToString() : "N/A";
                    var errMsg = root.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "Lỗi không xác định";

                    throw new UserFriendlyException($"Gửi SMS thất bại (Mã lỗi SpeedSMS {errCode}): {errMsg}");
                }
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;

            phone = phone.Trim();
            if (phone.StartsWith("0"))
            {
                return "84" + phone.Substring(1);
            }
            return phone;
        }
    }
}