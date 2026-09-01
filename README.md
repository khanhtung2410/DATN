# Cửa hàng Chăm sóc Thú cưng

Repository chứa backend API và giao diện web cho ứng dụng quản lý cửa hàng chăm sóc thú cưng (xây trên ABP + ASP.NET Core).

Tóm tắt
- Multi-project .NET 8: API, MVC/Razor Pages, EF Core, Migrator.
- Các tính năng chính: quản lý khách hàng, thú cưng, dịch vụ, lịch chăm sóc, hóa đơn, VIP, thông báo, thanh toán QR, gửi SMS.

Chức năng theo vai trò

- Khách hàng (Customer):
  - Đăng ký / Đăng nhập (JWT)
  - Quản lý thông tin cá nhân
  - Quản lý thú cưng (thêm, sửa, upload ảnh)
  - Xem danh sách dịch vụ và bảng giá
  - Đặt lịch chăm sóc cho thú cưng
  - Xem lịch của mình, hủy lịch
  - Xem hóa đơn, nhận QR thanh toán

- Quản trị (Admin / Staff):
  - Quản lý dịch vụ, bảng giá
  - Quản lý nhân viên
  - Duyệt / phân công / từ chối / hoàn thành lịch chăm sóc
  - Tạo hóa đơn từ lịch đã hoàn thành
  - Cấu hình VIP và chính sách giảm giá
  - Quản lý thông báo (push/notification)

Các endpoint chính theo chức năng (ví dụ theo pattern ABP proxy)

- Xác thực
  - `POST /api/TokenAuth/Authenticate` (body: `userNameOrEmailAddress`, `password`) -> trả về JWT

- Khách hàng
  - `POST /api/services/app/KhachHang/DangKy` (body: `hoTen, sdt, matKhau, xacNhanMatKhau, email?`)
  - `GET  /api/services/app/KhachHang/GetThongTinCaNhan` (auth)

- Thú cưng (ThuCung)
  - `GET  /api/services/app/ThuCung/GetAll`
  - `GET  /api/services/app/ThuCung/Get?id={id}`
  - `POST /api/services/app/ThuCung/Create` (body: `tenThuCung, loaiThuCung, ghiChu, trangThai`)
  - `PUT  /api/services/app/ThuCung/Update` (body: `id, tenThuCung, ...`)
  - `POST /api/services/app/ThuCung/UploadImage` (form-data: `thuCungId`, `file`)

- Lịch chăm sóc (LichChamSoc)
  - `GET  /api/services/app/LichChamSoc/GetAll` (params: tenKhachHang, trangThai, page, pageSize)
  - `GET  /api/services/app/LichChamSoc/GetLichChamSoc?id={id}`
  - `POST /api/services/app/LichChamSoc/Create` (body: `thuCungId, dichVuId, bangGiaId, thoiGian`)
  - `POST /api/services/app/LichChamSoc/HuyLichChamSoc` (body: `id`)
  - `POST /api/services/app/LichChamSoc/PhanCongNhanVien` (admin)
  - `POST /api/services/app/LichChamSoc/TuChoiLichChamSoc` (admin)

- Hóa đơn (HoaDon)
  - `GET  /api/services/app/HoaDon/GetAll`
  - `GET  /api/services/app/HoaDon/GetChiTiet?id={id}`
  - `POST /api/services/app/HoaDon/ThemHoaDon` (tạo từ lịch đã hoàn thành)
  - `POST /api/services/app/HoaDon/TaoQrThanhToan` (body: `hoaDonId`) -> trả về `UrlQr`
  - `POST /api/services/app/HoaDon/XacNhanThanhToan` (body: `id`) (admin)

- VIP
  - `GET  /api/services/app/Vip/LayDanhSachVip`
  - `GET  /api/services/app/Vip/Get?id={id}`
  - `POST /api/services/app/Vip/ThemVip` (admin)
  - `POST /api/services/app/Vip/ThemCauHinhVip` (admin)

- Hệ thống tích hợp
  - `SmsSettings` (SpeedSMS): cấu hình `SmsSettings:ApiUrl`, `SmsSettings:AccessToken`, `SmsSettings:DeviceID` — lớp gửi SMS: `SpeedSmsSender`
  - `VietQr` (QR thanh toán): cấu hình `VietQr:BankId, AccountNo, AccountName, Template` (dùng trong `HoaDonAppService.TaoQrThanhToan`)
  - `Techcombank` (nếu tích hợp cổng ngân hàng): mẫu cấu hình trong appsettings

Luồng chính: từ đặt lịch -> thanh toán

1) Khách hàng đăng ký / đăng nhập
   - `POST /api/services/app/KhachHang/DangKy`
   - `POST /api/TokenAuth/Authenticate` để lấy JWT

2) Khách hàng tạo lịch chăm sóc
   - Lấy danh sách dịch vụ/bảng giá: `GET /api/services/app/DichVu/GetAll`
   - Tạo lịch: `POST /api/services/app/LichChamSoc/Create` (gửi `thuCungId, dichVuId, bangGiaId, thoiGian`)

3) Admin/Staff xử lý lịch
   - Xem danh sách lịch chờ: `GET /api/services/app/LichChamSoc/GetAll` (filter `trangThai`)
   - Phân công nhân viên: `POST /api/services/app/LichChamSoc/PhanCongNhanVien` (admin)
   - Nếu từ chối: `POST /api/services/app/LichChamSoc/TuChoiLichChamSoc`
   - Khi hoàn thành: staff đánh dấu hoàn thành (API thay đổi trạng thái)

4) Tạo hóa đơn từ lịch đã hoàn thành
   - `POST /api/services/app/HoaDon/ThemHoaDon` (body: `lichChamSocId`) -> trả về `hoaDonId`

5) Tạo QR thanh toán (VietQR) và hiển thị cho khách
   - `POST /api/services/app/HoaDon/TaoQrThanhToan` (body: `hoaDonId`) -> trả về `UrlQr` (link ảnh QR)
   - Khách hàng dùng app ngân hàng quét QR để thanh toán hoặc dùng tích hợp Techcombank (tuỳ cấu hình)

6) Xác nhận thanh toán
   - Admin xác nhận hoặc hệ thống tự động chuyển trạng thái khi nhận webhook từ cổng thanh toán
   - `POST /api/services/app/HoaDon/XacNhanThanhToan` (body: `hoaDonId`)

Mẫu cấu hình `appsettings.json` (ví dụ, KHÔNG đặt giá trị thật vào repo)

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
  }
}
```

Chạy trên máy local
1. Cài .NET 8 SDK
2. Cập nhật cấu hình (file ví dụ):
   - `src/Cuahangchamsocthucung.Web.Host/appsettings.json`
   - `src/Cuahangchamsocthucung.Web.Mvc/appsettings.json`

Migrations
- Project migrator: `src/Cuahangchamsocthucung.Migrator`
- Chạy migration để tạo DB / cập nhật schema:
  - `dotnet run --project src/Cuahangchamsocthucung.Migrator/Cuahangchamsocthucung.Migrator.csproj`

Chạy ứng dụng
- API (Web.Host): `dotnet run --project src/Cuahangchamsocthucung.Web.Host/Cuahangchamsocthucung.Web.Host.csproj`
- MVC/Razor Pages (giao diện): `dotnet run --project src/Cuahangchamsocthucung.Web.Mvc/Cuahangchamsocthucung.Web.Mvc.csproj`

Bảo mật
- Tuyệt đối không commit các giá trị thực tế: `AccessToken`, `SecurityKey`, mật khẩu DB, keys của bên thứ ba.
- Dùng `appsettings.Development.json` (git-ignored), biến môi trường hoặc `dotnet user-secrets` cho dev.

Ghi chú
- README tập trung vào chức năng và luồng chính. Nếu muốn bổ sung hướng dẫn triển khai production, Docker, hoặc chi tiết webhook/payment flow (Techcombank), mình có thể thêm.
