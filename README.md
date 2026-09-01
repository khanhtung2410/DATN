# C?a hàng Ch?m sóc Thú c?ng

Repository này ch?a backend API và giao di?n web cho ?ng d?ng qu?n lý c?a hàng ch?m sóc thú c?ng (d? án xây d?ng trên n?n ABP + ASP.NET Core).

Tóm t?t nhanh
- ?a d? án .NET 8 (API, MVC, EF Core, Migrator, v.v.)
- Các tính n?ng chính: qu?n lý khách hàng, thú c?ng, d?ch v?, l?ch ch?m sóc, hóa ??n, VIP, thông báo, thanh toán QR, g?i SMS.

API chính
- Token authentication: `POST /api/TokenAuth/Authenticate`
- D?ch v? ABP (proxy) (ví d?):
  - `GET/POST/PUT/DELETE /api/services/app/ThuCung` (qu?n lý thú c?ng)
  - `GET/POST /api/services/app/LichChamSoc` (l?ch ch?m sóc)
  - `GET/POST /api/services/app/HoaDon` (hóa ??n)
  - `GET/POST /api/services/app/Vip` (VIP)
  - `GET/POST /api/services/app/KhachHang` (khách hàng)
  - `GET/POST /api/services/app/DichVu` (d?ch v?)

Giao di?n Swagger
- Sau khi ch?y ?ng d?ng host, truy c?p: `http://localhost:{port}/swagger` ?? xem và th? các API.
- README không ch?a token/bí m?t; s? d?ng endpoint `Authenticate` ?? l?y token (JWT) và l?u vào header `Authorization: Bearer {token}` khi g?i API.

Ch?y trên máy local
1. Cài .NET 8 SDK
2. C?p nh?t c?u hình
   - Các file c?u hình chính (không nên commit bí m?t) n?m trong:
     - `src/Cuahangchamsocthucung.Web.Host/appsettings.json`
     - `src/Cuahangchamsocthucung.Web.Mvc/appsettings.json`
   - M?t s? khóa c?u hình c?n c?p nh?t (?ây là m?u — ch? dùng giá tr? ví d?, KHÔNG ??t khóa th?t trong repo):

```json
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=YOUR_DB_NAME;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;" 
  },
  "App": {
    "CorsOrigins": "https://localhost:4200,https://localhost:5001"
  },
  "Authentication": {
    "JwtBearer": {
      "SecurityKey": "YOUR_JWT_SECURITY_KEY_SHOULD_BE_LONG",
      "Issuer": "YOUR_ISSUER",
      "Audience": "YOUR_AUDIENCE"
    }
  },
  "SmsSettings": {
    "ApiUrl": "https://api.speedsms.vn/v2/sms/bulk",
    "AccessToken": "API_KEY",
    "DeviceID": "DEVICE_ID"
  },
  "VietQr": {
    "BankId": "VCB",
    "AccountNo": "0123456789",
    "AccountName": "TEN_TAI_KHOAN",
    "Template": "YOUR_TEMPLATE" 
  },
  "Techcombank": {
    "ApiKey": "API_KEY",
    "ApiSecret": "API_SECRET",
    "TerminalId": "TERMINAL_ID"
  },
  "Swagger": {
    "ShowSummaries": true
  }
}
```

L?u ý c?u hình:
- `SmsSettings` t??ng ?ng v?i l?p g?i SMS (`SpeedSmsSender`).
- `VietQr` dùng ?? t?o link QR thanh toán (xem `HoaDonAppService.TaoQrThanhToan`).
- `Techcombank` n?u tích h?p c?ng thanh toán ngân hàng thì thêm thông tin c?n thi?t (ví d? API key/secret/terminal id).
- Không l?u thông tin nh?y c?m (API keys, m?t kh?u, connection strings) trong kho chung — dùng `appsettings.Development.json` (git-ignored), bi?n môi tr??ng ho?c user-secrets.

Migrations (c? s? d? li?u)
- Project migrator: `src/Cuahangchamsocthucung.Migrator`
- Ch?y migration ?? t?o DB / c?p nh?t schema:
  - dotnet run --project src/Cuahangchamsocthucung.Migrator/Cuahangchamsocthucung.Migrator.csproj

Ch?y ?ng d?ng API
- Ch?y Web.Host (API + Swagger):
  - dotnet run --project src/Cuahangchamsocthucung.Web.Host/Cuahangchamsocthucung.Web.Host.csproj

Ch?y front-end MVC
- Ch?y Web.Mvc (n?u c?n):
  - dotnet run --project src/Cuahangchamsocthucung.Web.Mvc/Cuahangchamsocthucung.Web.Mvc.csproj

Thông tin b?o m?t
- Tuy?t ??i không commit các giá tr? th?c t? c?a `AccessToken`, `SecurityKey`, m?t kh?u DB, ho?c keys c?a bên th? ba.
- Trong README này ch? ch?a ví d? placeholder nh? `API_KEY`. Th?c t? hãy dùng bi?n môi tr??ng, file `appsettings.Development.json` ho?c `dotnet user-secrets` trong môi tr??ng dev.

Debug / Logs
- Log4net ???c c?u hình trong `Web.Host` (t?p `log4net.config` / `log4net.Production.config`).

Góp ý & ?óng góp
- M?i ?óng góp vui lòng t?o PR trên GitHub. Không thêm secrets vào PR/public commits.

Các tài nguyên khác
- Swagger UI: `/swagger`
- ABP proxy API pattern: `/api/services/app/{ServiceName}/{Method}`

Liên h?
- Thêm h??ng d?n n?i b?, tài li?u chi ti?t h?n tùy theo nhu c?u tri?n khai — n?u mu?n mình có th? b? sung ph?n h??ng d?n tri?n khai trên môi tr??ng production, Docker ho?c Azure.
