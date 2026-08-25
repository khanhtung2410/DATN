using Abp;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using Cuahangchamsocthucung.Authorization;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Enum;
using Cuahangchamsocthucung.LichChamSoc.Dto;
using Cuahangchamsocthucung.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LichChamSocAppService : ApplicationService, ILichChamSocAppService
{
    private readonly IRepository<LichChamSoc> _lichChamSocRepository;
    private readonly IRepository<KhachHang> _khachHangRepository;
    private readonly IRepository<ThuCung> _thuCungRepository;
    private readonly IRepository<DichVu> _dichVuRepository;
    private readonly IRepository<BangGia> _bangGiaRepository;
    private readonly IRepository<NhanVien> _nhanVienRepository;
    private readonly INotificationPublisher _notificationPublisher;

    public LichChamSocAppService(
        IRepository<LichChamSoc> lichChamSocRepository,
        IRepository<KhachHang> khachHangRepository,
        IRepository<ThuCung> thuCungRepository,
        IRepository<DichVu> dichVuRepository,
        IRepository<BangGia> bangGiaRepository,
        IRepository<NhanVien> nhanVienRepository,
        INotificationPublisher notificationPublisher)
    {
        _lichChamSocRepository = lichChamSocRepository;
        _khachHangRepository = khachHangRepository;
        _thuCungRepository = thuCungRepository;
        _dichVuRepository = dichVuRepository;
        _bangGiaRepository = bangGiaRepository;
        _nhanVienRepository = nhanVienRepository;
        _notificationPublisher = notificationPublisher;
    }

    [AbpAuthorize]
    public async Task<LichChamSocDto> GetLichChamSoc(int id)
    {
        var result = await _lichChamSocRepository.GetAll()
            .Where(x => x.Id == id)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                ThuCungId = x.ThuCungId,
                TenThuCung = x.ThuCung != null ? x.ThuCung.TenThuCung : string.Empty,
                DichVuId = x.DichVuId,
                TenDichVu = x.DichVu != null ? x.DichVu.TenDichVu : string.Empty,
                BangGiaId = x.BangGiaId,
                NhanVienId = x.NhanVienId,
                TenNhanVien = x.NhanVien != null ? x.NhanVien.Hoten : string.Empty,
                KhachHangId = x.KhachHangId,
                TenKhachHang = x.KhachHang != null ? x.KhachHang.Hoten : string.Empty,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync();

        if (result == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        return result;
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task<PagedResultDto<LichChamSocDto>> GetAll(
     string tenKhachHang = "",
     TrangThaiLichChamSoc? trangThai = null,
     int page = 1,
     int pageSize = 10)
    {
        var query = _lichChamSocRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(tenKhachHang))
        {
            tenKhachHang = tenKhachHang.Trim();

            query = query.Where(x =>
                x.KhachHang != null &&
                x.KhachHang.Hoten.Contains(tenKhachHang));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(x =>
                x.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.ThoiGian)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,

                ThuCungId = x.ThuCungId,

                TenThuCung = x.ThuCung != null
                    ? x.ThuCung.TenThuCung
                    : string.Empty,

                DichVuId = x.DichVuId,

                TenDichVu = x.DichVu != null
                    ? x.DichVu.TenDichVu
                    : string.Empty,

                BangGiaId = x.BangGiaId,

                NhanVienId = x.NhanVienId,

                TenNhanVien = x.NhanVien != null
                    ? x.NhanVien.Hoten
                    : string.Empty,

                KhachHangId = x.KhachHangId,

                TenKhachHang = x.KhachHang != null
                    ? x.KhachHang.Hoten
                    : string.Empty,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .ToListAsync();

        return new PagedResultDto<LichChamSocDto>(
            totalCount,
            items
        );
    }
    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task Update(SuaLichChamSocDto input)
    {
        if (input == null)
            throw new UserFriendlyException("Dữ liệu lịch chăm sóc không hợp lệ.");

        var lichChamSoc = await _lichChamSocRepository.FirstOrDefaultAsync(input.Id);

        if (lichChamSoc == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.ChoXacNhan &&
            lichChamSoc.TrangThai != TrangThaiLichChamSoc.DaXacNhan)
            throw new UserFriendlyException(
                "Chỉ có thể sửa lịch đang chờ xác nhận hoặc đã xác nhận.");

        if (input.DichVuId <= 0)
            throw new UserFriendlyException("Vui lòng chọn dịch vụ.");

        if (input.BangGiaId <= 0)
            throw new UserFriendlyException("Vui lòng chọn bảng giá.");

        if (input.ThoiGian == default)
            throw new UserFriendlyException("Vui lòng chọn thời gian.");

        if (input.ThoiGian <= DateTime.Now)
            throw new UserFriendlyException("Không thể chọn thời gian đã qua.");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x =>
            x.Id == input.BangGiaId &&
            x.DichVuId == input.DichVuId);

        if (bangGia == null)
            throw new UserFriendlyException("Bảng giá không thuộc dịch vụ đã chọn.");

        if (bangGia.ThoiGianPhut <= 0)
            throw new UserFriendlyException("Thời lượng dịch vụ không hợp lệ.");

        var dichVu = await _dichVuRepository.FirstOrDefaultAsync(x =>
            x.Id == input.DichVuId &&
            x.TrangThai);

        if (dichVu == null)
            throw new UserFriendlyException(
                "Dịch vụ không tồn tại hoặc hiện không hoạt động.");

        if (input.NhanVienId.HasValue)
        {
            var thoiGianBatDau = input.ThoiGian;
            var thoiGianKetThuc =
                thoiGianBatDau.AddMinutes(bangGia.ThoiGianPhut);

            var lichTrung = await _lichChamSocRepository.GetAll()
                .Where(x =>
                    x.Id != input.Id &&
                    x.NhanVienId == input.NhanVienId.Value &&
                    x.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                    x.TrangThai != TrangThaiLichChamSoc.BiTuChoi)
                .Join(
                    _bangGiaRepository.GetAll(),
                    lichKhac => lichKhac.BangGiaId,
                    bg => bg.Id,
                    (lichKhac, bg) => new
                    {
                        Lich = lichKhac,
                        BangGia = bg
                    })
                .AnyAsync(x =>
                    thoiGianBatDau <
                        x.Lich.ThoiGian.AddMinutes(x.BangGia.ThoiGianPhut)
                    &&
                    thoiGianKetThuc >
                        x.Lich.ThoiGian);

            if (lichTrung)
                throw new UserFriendlyException(
                    "Nhân viên đã có lịch chăm sóc khác trong khung giờ này.");
        }

        lichChamSoc.DichVuId = input.DichVuId;
        lichChamSoc.BangGiaId = input.BangGiaId;
        lichChamSoc.ThoiGian = input.ThoiGian;

        if (input.NhanVienId.HasValue)
            lichChamSoc.NhanVienId = input.NhanVienId.Value;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [AbpAuthorize]
    public async Task ChangeStatus(SuaTrangThaiLichChamSocDto input)
    {
        if (input == null)
            throw new UserFriendlyException("Dữ liệu trạng thái không hợp lệ.");

        var lichChamSoc =
            await _lichChamSocRepository.FirstOrDefaultAsync(input.Id);

        if (lichChamSoc == null)
            throw new UserFriendlyException("Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai == TrangThaiLichChamSoc.HoanThanh ||
            lichChamSoc.TrangThai == TrangThaiLichChamSoc.DaHuy ||
            lichChamSoc.TrangThai == TrangThaiLichChamSoc.BiTuChoi)
            throw new UserFriendlyException(
                "Không thể thay đổi trạng thái lịch này.");

        lichChamSoc.TrangThai = input.TrangThai;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [AbpAuthorize]
    public async Task<int> Create(ThemLichChamSocDto input)
    {
        if (input == null)
            throw new UserFriendlyException("Dữ liệu đặt lịch không hợp lệ.");

        if (input.ThuCungId <= 0)
            throw new UserFriendlyException("Vui lòng chọn thú cưng.");

        if (input.DichVuId <= 0)
            throw new UserFriendlyException("Vui lòng chọn dịch vụ.");

        if (input.BangGiaId <= 0)
            throw new UserFriendlyException("Vui lòng chọn bảng giá.");

        if (input.ThoiGian == default)
            throw new UserFriendlyException("Vui lòng chọn ngày và giờ.");

        if (input.ThoiGian <= DateTime.Now)
            throw new UserFriendlyException(
                "Không thể đặt lịch ở thời gian đã qua.");

        var userId = AbpSession.UserId;

        if (!userId.HasValue)
            throw new UserFriendlyException(
                "Vui lòng đăng nhập để đặt lịch.");

        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x =>
            x.UserId == userId.Value);

        if (khachHang == null)
            throw new UserFriendlyException(
                "Không tìm thấy thông tin khách hàng.");

        var thuCung = await _thuCungRepository.FirstOrDefaultAsync(x =>
            x.Id == input.ThuCungId &&
            x.KhachHangId == khachHang.Id &&
            x.TrangThai);

        if (thuCung == null)
            throw new UserFriendlyException(
                "Thú cưng không thuộc tài khoản của bạn hoặc đã ngừng hoạt động.");

        var dichVu = await _dichVuRepository.FirstOrDefaultAsync(x =>
            x.Id == input.DichVuId &&
            x.TrangThai);

        if (dichVu == null)
            throw new UserFriendlyException(
                "Dịch vụ không tồn tại hoặc hiện không hoạt động.");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x =>
            x.Id == input.BangGiaId &&
            x.DichVuId == input.DichVuId);

        if (bangGia == null)
            throw new UserFriendlyException(
                "Bảng giá không thuộc dịch vụ đã chọn.");

        if (bangGia.ThoiGianPhut <= 0)
            throw new UserFriendlyException(
                "Thời lượng dịch vụ không hợp lệ.");

        var thoiGianKetThuc =
            input.ThoiGian.AddMinutes(bangGia.ThoiGianPhut);

        var lichTrung = await _lichChamSocRepository.GetAll()
            .Join(
                _bangGiaRepository.GetAll(),
                lich => lich.BangGiaId,
                bg => bg.Id,
                (lich, bg) => new
                {
                    Lich = lich,
                    BangGia = bg
                })
            .AnyAsync(x =>
                x.Lich.ThuCungId == input.ThuCungId &&
                x.Lich.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                x.Lich.TrangThai != TrangThaiLichChamSoc.BiTuChoi &&
                input.ThoiGian <
                    x.Lich.ThoiGian.AddMinutes(x.BangGia.ThoiGianPhut)
                &&
                thoiGianKetThuc >
                    x.Lich.ThoiGian);

        if (lichTrung)
            throw new UserFriendlyException(
                "Thú cưng đã có lịch chăm sóc trùng với khung giờ này.");

        var lichChamSoc = new LichChamSoc
        {
            ThuCungId = input.ThuCungId,
            DichVuId = input.DichVuId,
            BangGiaId = input.BangGiaId,
            KhachHangId = khachHang.Id,
            NhanVienId = null,
            ThoiGian = input.ThoiGian,
            TrangThai = TrangThaiLichChamSoc.ChoXacNhan
        };

        var id =
            await _lichChamSocRepository.InsertAndGetIdAsync(lichChamSoc);

        await CurrentUnitOfWork.SaveChangesAsync();

        return id;
    }

    [AbpAuthorize]
    public async Task<List<LichChamSocDto>> GetLichChamSocCuaToi()
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
            throw new UserFriendlyException("Vui lòng đăng nhập.");

        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x =>
            x.UserId == userId.Value);

        if (khachHang == null)
            throw new UserFriendlyException(
                "Không tìm thấy khách hàng.");

        return await _lichChamSocRepository.GetAll()
            .Where(x => x.KhachHangId == khachHang.Id)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                ThuCungId = x.ThuCungId,
                DichVuId = x.DichVuId,
                BangGiaId = x.BangGiaId,
                NhanVienId = x.NhanVienId,
                KhachHangId = x.KhachHangId,
                ThoiGian = x.ThoiGian,
                TenThuCung = x.ThuCung != null
                    ? x.ThuCung.TenThuCung
                    : string.Empty,
                TenDichVu = x.DichVu != null
                    ? x.DichVu.TenDichVu
                    : string.Empty,
                TenNhanVien = x.NhanVien != null
                    ? x.NhanVien.Hoten
                    : string.Empty,
                TrangThai = x.TrangThai
            })
            .OrderBy(x => x.ThoiGian)
            .ToListAsync();
    }

    [AbpAuthorize]
    public async Task<List<LichChamSocDto>> GetLichSuLichChamSocCuaToi()
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
            return new List<LichChamSocDto>();

        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x =>
            x.UserId == userId.Value);

        if (khachHang == null)
            return new List<LichChamSocDto>();

        return await _lichChamSocRepository.GetAll()
            .Where(x =>
                x.KhachHangId == khachHang.Id &&
                (x.TrangThai == TrangThaiLichChamSoc.HoanThanh ||
                 x.TrangThai == TrangThaiLichChamSoc.BiTuChoi ||
                 x.TrangThai == TrangThaiLichChamSoc.DaHuy))
            .OrderByDescending(x => x.ThoiGian)
            .Select(x => new LichChamSocDto
            {
                Id = x.Id,
                KhachHangId = x.KhachHangId,
                NhanVienId = x.NhanVienId,
                DichVuId = x.DichVuId,
                BangGiaId = x.BangGiaId,
                ThuCungId = x.ThuCungId,
                TenThuCung = x.ThuCung != null
                    ? x.ThuCung.TenThuCung
                    : string.Empty,
                TenDichVu = x.DichVu != null
                    ? x.DichVu.TenDichVu
                    : string.Empty,
                TenNhanVien = x.NhanVien != null
                    ? x.NhanVien.Hoten
                    : string.Empty,
                ThoiGian = x.ThoiGian,
                TrangThai = x.TrangThai
            })
            .ToListAsync();
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task PhanCongNhanVien(
        int lichChamSocId,
        int nhanVienId)
    {
        if (lichChamSocId <= 0)
            throw new UserFriendlyException(
                "Lịch chăm sóc không hợp lệ.");

        if (nhanVienId <= 0)
            throw new UserFriendlyException(
                "Nhân viên không hợp lệ.");

        var lich =
            await _lichChamSocRepository.FirstOrDefaultAsync(lichChamSocId);

        if (lich == null)
            throw new UserFriendlyException(
                "Lịch chăm sóc không tồn tại.");

        if (lich.TrangThai != TrangThaiLichChamSoc.ChoXacNhan)
            throw new UserFriendlyException(
                "Chỉ có thể phân công nhân viên cho lịch đang chờ xác nhận.");

        var nhanVien = await _nhanVienRepository.FirstOrDefaultAsync(x =>
            x.Id == nhanVienId &&
            x.Trangthai);

        if (nhanVien == null)
            throw new UserFriendlyException(
                "Nhân viên không tồn tại hoặc không còn làm việc.");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x =>
            x.Id == lich.BangGiaId);

        if (bangGia == null)
            throw new UserFriendlyException(
                "Không tìm thấy bảng giá của lịch chăm sóc.");

        if (bangGia.ThoiGianPhut <= 0)
            throw new UserFriendlyException(
                "Thời lượng dịch vụ không hợp lệ.");

        var thoiGianBatDau = lich.ThoiGian;

        var thoiGianKetThuc =
            thoiGianBatDau.AddMinutes(bangGia.ThoiGianPhut);

        var lichTrung = await _lichChamSocRepository.GetAll()
            .Where(x =>
                x.Id != lichChamSocId &&
                x.NhanVienId == nhanVienId &&
                x.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                x.TrangThai != TrangThaiLichChamSoc.BiTuChoi)
            .Join(
                _bangGiaRepository.GetAll(),
                lichKhac => lichKhac.BangGiaId,
                bg => bg.Id,
                (lichKhac, bg) => new
                {
                    Lich = lichKhac,
                    BangGia = bg
                })
            .AnyAsync(x =>
                thoiGianBatDau <
                    x.Lich.ThoiGian.AddMinutes(x.BangGia.ThoiGianPhut)
                &&
                thoiGianKetThuc >
                    x.Lich.ThoiGian);

        if (lichTrung)
            throw new UserFriendlyException(
                "Nhân viên đã có lịch chăm sóc khác trong khung giờ này.");

        lich.NhanVienId = nhanVienId;
        lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;

        await _lichChamSocRepository.UpdateAsync(lich);
        await CurrentUnitOfWork.SaveChangesAsync();
        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(
    x => x.Id == lich.KhachHangId);

        if (khachHang != null && khachHang.UserId > 0)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.LichChamSocDaXacNhan,
                new MessageNotificationData(
                    $"Lịch chăm sóc ngày {lich.ThoiGian:dd/MM/yyyy} " +
                    $"lúc {lich.ThoiGian:HH:mm} " +
                    $"đã được cửa hàng xác nhận. " +
                    $"Nhân viên thực hiện: {nhanVien.Hoten}."
                ),
                null,
                userIds: new[]
                {
            new UserIdentifier(
                khachHang.TenantId,
                khachHang.UserId)
                });
        }
    }

    // ============================================================
    // TỰ ĐỘNG PHÂN CÔNG NHÂN VIÊN
    // ============================================================
    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task<string> TuDongPhanCongNhanVien(int lichChamSocId)
    {
        if (lichChamSocId <= 0)
            throw new UserFriendlyException(
                "Lịch chăm sóc không hợp lệ.");

        var lich =
            await _lichChamSocRepository.FirstOrDefaultAsync(lichChamSocId);

        if (lich == null)
            throw new UserFriendlyException(
                "Lịch chăm sóc không tồn tại.");

        if (lich.TrangThai != TrangThaiLichChamSoc.ChoXacNhan)
            throw new UserFriendlyException(
                "Chỉ có thể tự động phân công lịch đang chờ xác nhận.");

        if (lich.NhanVienId.HasValue)
            throw new UserFriendlyException(
                "Lịch chăm sóc đã được phân công nhân viên.");

        var bangGia = await _bangGiaRepository.FirstOrDefaultAsync(x =>
            x.Id == lich.BangGiaId);

        if (bangGia == null)
            throw new UserFriendlyException(
                "Không tìm thấy bảng giá của lịch chăm sóc.");

        if (bangGia.ThoiGianPhut <= 0)
            throw new UserFriendlyException(
                "Thời lượng dịch vụ không hợp lệ.");

        var thoiGianBatDau = lich.ThoiGian;

        var thoiGianKetThuc =
            thoiGianBatDau.AddMinutes(bangGia.ThoiGianPhut);

        var nhanViens = await _nhanVienRepository.GetAll()
            .Where(x => x.Trangthai)
            .OrderBy(x => x.Id)
            .ToListAsync();

        if (!nhanViens.Any())
            throw new UserFriendlyException(
                "Không có nhân viên đang làm việc.");

        var ungVien = new List<NhanVien>();

        foreach (var nhanVien in nhanViens)
        {
            var biTrung = await _lichChamSocRepository.GetAll()
                .Where(x =>
                    x.Id != lichChamSocId &&
                    x.NhanVienId == nhanVien.Id &&
                    x.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                    x.TrangThai != TrangThaiLichChamSoc.BiTuChoi)
                .Join(
                    _bangGiaRepository.GetAll(),
                    lichKhac => lichKhac.BangGiaId,
                    bg => bg.Id,
                    (lichKhac, bg) => new
                    {
                        Lich = lichKhac,
                        BangGia = bg
                    })
                .AnyAsync(x =>
                    thoiGianBatDau <
                        x.Lich.ThoiGian.AddMinutes(x.BangGia.ThoiGianPhut)
                    &&
                    thoiGianKetThuc >
                        x.Lich.ThoiGian);

            if (!biTrung)
                ungVien.Add(nhanVien);
        }

        if (!ungVien.Any())
            throw new UserFriendlyException(
                "Không có nhân viên nào trống trong khung giờ này.");

        // Ưu tiên nhân viên có ít lịch nhất trong ngày
        var ngayBatDau = lich.ThoiGian.Date;
        var ngayKetThuc = ngayBatDau.AddDays(1);

        var lichTrongNgay = await _lichChamSocRepository.GetAll()
            .Where(x =>
                x.NhanVienId.HasValue &&
                x.ThoiGian >= ngayBatDau &&
                x.ThoiGian < ngayKetThuc &&
                x.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                x.TrangThai != TrangThaiLichChamSoc.BiTuChoi)
            .GroupBy(x => x.NhanVienId.Value)
            .Select(g => new
            {
                NhanVienId = g.Key,
                SoLuongLich = g.Count()
            })
            .ToListAsync();

        var nhanVienDuocChon = ungVien
            .OrderBy(nv =>
                lichTrongNgay
                    .FirstOrDefault(x => x.NhanVienId == nv.Id)
                    ?.SoLuongLich ?? 0)
            .ThenBy(nv => nv.Id)
            .First();

        lich.NhanVienId = nhanVienDuocChon.Id;
        lich.TrangThai = TrangThaiLichChamSoc.DaXacNhan;

        await _lichChamSocRepository.UpdateAsync(lich);
        await CurrentUnitOfWork.SaveChangesAsync();
        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(
    x => x.Id == lich.KhachHangId);

        if (khachHang != null && khachHang.UserId > 0)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.LichChamSocDaXacNhan,
                new MessageNotificationData(
                    $"Lịch chăm sóc ngày {lich.ThoiGian:dd/MM/yyyy} " +
                    $"lúc {lich.ThoiGian:HH:mm} " +
                    $"đã được cửa hàng xác nhận. " +
                    $"Nhân viên thực hiện: {nhanVienDuocChon.Hoten}."
                ),
                null,
                userIds: new[]
                {
            new UserIdentifier(
                khachHang.TenantId,
                khachHang.UserId)
                });
        }
        return nhanVienDuocChon.Hoten;
    }

    [AbpAuthorize]
    public async Task HuyLichChamSoc(int id)
    {
        var userId = AbpSession.UserId;

        if (!userId.HasValue)
            throw new UserFriendlyException(
                "Vui lòng đăng nhập.");

        var khachHang = await _khachHangRepository.FirstOrDefaultAsync(x =>
            x.UserId == userId.Value);

        if (khachHang == null)
            throw new UserFriendlyException(
                "Không tìm thấy khách hàng.");

        var lichChamSoc = await _lichChamSocRepository.FirstOrDefaultAsync(x =>
            x.Id == id &&
            x.KhachHangId == khachHang.Id);

        if (lichChamSoc == null)
            throw new UserFriendlyException(
                "Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.ChoXacNhan &&
            lichChamSoc.TrangThai != TrangThaiLichChamSoc.DaXacNhan)
            throw new UserFriendlyException(
                "Chỉ được hủy lịch đang chờ xác nhận hoặc đã xác nhận.");

        lichChamSoc.TrangThai = TrangThaiLichChamSoc.DaHuy;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task TuChoiLichChamSoc(int id)
    {
        var lichChamSoc =
            await _lichChamSocRepository.FirstOrDefaultAsync(id);

        if (lichChamSoc == null)
            throw new UserFriendlyException(
                "Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.ChoXacNhan &&
            lichChamSoc.TrangThai != TrangThaiLichChamSoc.DaXacNhan)
            throw new UserFriendlyException(
                "Chỉ có thể từ chối lịch đang chờ xác nhận hoặc đã xác nhận.");

        lichChamSoc.TrangThai =
            TrangThaiLichChamSoc.BiTuChoi;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();

        var khachHang =
            await _khachHangRepository.FirstOrDefaultAsync(
                x => x.Id == lichChamSoc.KhachHangId);

        if (khachHang != null && khachHang.UserId > 0)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.LichChamSocBiTuChoi,
                new MessageNotificationData(
                    $"Lịch chăm sóc ngày {lichChamSoc.ThoiGian:dd/MM/yyyy} " +
                    $"lúc {lichChamSoc.ThoiGian:HH:mm} " +
                    $"đã bị cửa hàng từ chối."
                ),
                null,
                userIds: new[]
                {
                    new UserIdentifier(
                        khachHang.TenantId,
                        khachHang.UserId)
                });
        }
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task HoanThanhLichChamSoc(int id)
    {
        var lichChamSoc =
            await _lichChamSocRepository.FirstOrDefaultAsync(id);

        if (lichChamSoc == null)
            throw new UserFriendlyException(
                "Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.DangDienRa)
            throw new UserFriendlyException(
                "Chỉ có thể hoàn thành lịch đang diễn ra.");

        lichChamSoc.TrangThai =
            TrangThaiLichChamSoc.HoanThanh;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();

        var khachHang =
            await _khachHangRepository.FirstOrDefaultAsync(
                x => x.Id == lichChamSoc.KhachHangId);

        if (khachHang != null && khachHang.UserId > 0)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.LichChamSocDaHoanThanh,
                new MessageNotificationData(
                    $"Lịch chăm sóc ngày {lichChamSoc.ThoiGian:dd/MM/yyyy} " +
                    $"lúc {lichChamSoc.ThoiGian:HH:mm} " +
                    $"đã được hoàn thành."
                ),
                null,
                userIds: new[]
                {
                    new UserIdentifier(
                        khachHang.TenantId,
                        khachHang.UserId)
                });
        }
    }

    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task BatDauViec(int id)
    {
        var lichChamSoc =
            await _lichChamSocRepository.FirstOrDefaultAsync(id);

        if (lichChamSoc == null)
            throw new UserFriendlyException(
                "Lịch chăm sóc không tồn tại.");

        if (lichChamSoc.TrangThai != TrangThaiLichChamSoc.DaXacNhan)
            throw new UserFriendlyException(
                "Chỉ có thể bắt đầu lịch đã được xác nhận.");

        if (!lichChamSoc.NhanVienId.HasValue)
            throw new UserFriendlyException(
                "Lịch chăm sóc chưa được phân công nhân viên.");

        lichChamSoc.TrangThai =
            TrangThaiLichChamSoc.DangDienRa;

        await _lichChamSocRepository.UpdateAsync(lichChamSoc);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
    [AbpAuthorize(PermissionNames.Pages_LichChamSoc)]
    public async Task<List<LichChamSocTimelineDto>> GetTimelineTrongNgay(DateTime ngay)
    {
        var ngayBatDau = ngay.Date;
        var ngayKetThuc = ngayBatDau.AddDays(1);

        return await _lichChamSocRepository.GetAll()
            .AsNoTracking()
            .Where(x =>
                x.ThoiGian >= ngayBatDau &&
                x.ThoiGian < ngayKetThuc &&
                x.TrangThai != TrangThaiLichChamSoc.DaHuy &&
                x.TrangThai != TrangThaiLichChamSoc.BiTuChoi)
            .Select(x => new LichChamSocTimelineDto
            {
                Id = x.Id,

                NhanVienId = x.NhanVienId,

                TenNhanVien = x.NhanVien != null
                    ? x.NhanVien.Hoten
                    : "Chưa phân công",

                TenThuCung = x.ThuCung != null
                    ? x.ThuCung.TenThuCung
                    : "",

                TenDichVu = x.DichVu != null
                    ? x.DichVu.TenDichVu
                    : "",

                TenKhachHang = x.KhachHang != null
                    ? x.KhachHang.Hoten
                    : "",

                ThoiGian = x.ThoiGian,

                ThoiGianPhut = x.BangGia.ThoiGianPhut,

                TrangThai = x.TrangThai
            })
            .OrderBy(x => x.ThoiGian)
            .ToListAsync();
    }
}