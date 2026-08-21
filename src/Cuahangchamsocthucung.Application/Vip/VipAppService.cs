using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.Vip.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Vip
{
    public class VipAppService : ApplicationService, IVipAppService
    {
        private readonly IRepository<Cuahangchamsocthucung.Entities.Vip, int> _vipRepository;
        private readonly IRepository<CauHinhVip, int> _cauHinhVipRepository;
        private readonly IRepository<Cuahangchamsocthucung.Entities.KhachHang, int> _khachHangRepository;

        public VipAppService(
            IRepository<Cuahangchamsocthucung.Entities.Vip, int> vipRepository,
            IRepository<CauHinhVip, int> cauHinhVipRepository,
            IRepository<Cuahangchamsocthucung.Entities.KhachHang, int> khachHangRepository)
        {
            _vipRepository = vipRepository;
            _cauHinhVipRepository = cauHinhVipRepository;
            _khachHangRepository = khachHangRepository;
        }

        // =====================================================
        // DANH SÁCH VIP
        // =====================================================
        public async Task<List<VipDto>> LayDanhSachVip()
        {
            return await _vipRepository
                .GetAll()
                .OrderBy(x => x.CapVip)
                .Select(x => new VipDto
                {
                    Id = x.Id,
                    CapVip = x.CapVip,
                    TenVip = x.TenVip
                })
                .ToListAsync();
        }

        // =====================================================
        // LẤY VIP THEO ID
        // =====================================================
        public async Task<VipDto> GetAsync(int id)
        {
            var vip = await _vipRepository.FirstOrDefaultAsync(id);

            if (vip == null)
                throw new UserFriendlyException("Không tìm thấy cấp VIP.");

            return new VipDto
            {
                Id = vip.Id,
                CapVip = vip.CapVip,
                TenVip = vip.TenVip
            };
        }

        // =====================================================
        // THÊM VIP
        // =====================================================
        public async Task<int> ThemVip(ThemVipDto input)
        {
            if (input == null)
                throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin VIP.");

            if (input.CapVip < 1 || input.CapVip > 5)
                throw new UserFriendlyException("Cấp VIP phải từ 1 đến 5.");

            if (string.IsNullOrWhiteSpace(input.TenVip))
                throw new UserFriendlyException("Tên VIP không được để trống.");

            var tonTaiCap = await _vipRepository
                .GetAll()
                .AnyAsync(x => x.CapVip == input.CapVip);

            if (tonTaiCap)
                throw new UserFriendlyException("Cấp VIP này đã tồn tại.");

            var vip = new Cuahangchamsocthucung.Entities.Vip
            {
                TenantId = AbpSession.GetTenantId(),
                CapVip = input.CapVip,
                TenVip = input.TenVip.Trim()
            };

            return await _vipRepository.InsertAndGetIdAsync(vip);
        }

        // =====================================================
        // SỬA VIP
        // =====================================================
        public async Task SuaVip(SuaVipDto input)
        {
            if (input == null || input.Id <= 0)
                throw new UserFriendlyException("Thông tin VIP không hợp lệ.");

            if (input.CapVip < 1 || input.CapVip > 5)
                throw new UserFriendlyException("Cấp VIP phải từ 1 đến 5.");

            if (string.IsNullOrWhiteSpace(input.TenVip))
                throw new UserFriendlyException("Tên VIP không được để trống.");

            var vip = await _vipRepository.FirstOrDefaultAsync(input.Id);

            if (vip == null)
                throw new UserFriendlyException("Không tìm thấy cấp VIP.");

            var tonTaiCap = await _vipRepository
                .GetAll()
                .AnyAsync(x =>
                    x.Id != input.Id &&
                    x.CapVip == input.CapVip);

            if (tonTaiCap)
                throw new UserFriendlyException("Cấp VIP này đã tồn tại.");

            vip.CapVip = input.CapVip;
            vip.TenVip = input.TenVip.Trim();

            await _vipRepository.UpdateAsync(vip);
        }

        // =====================================================
        // XÓA VIP
        // =====================================================
        public async Task XoaVip(int id)
        {
            var vip = await _vipRepository.FirstOrDefaultAsync(id);

            if (vip == null)
                throw new UserFriendlyException("Không tìm thấy cấp VIP.");

            var dangSuDung = await _khachHangRepository
                .GetAll()
                .AnyAsync(x => x.VipId == id);

            if (dangSuDung)
                throw new UserFriendlyException(
                    "Không thể xóa VIP đang được khách hàng sử dụng.");

            var coCauHinh = await _cauHinhVipRepository
                .GetAll()
                .AnyAsync(x => x.VipId == id);

            if (coCauHinh)
                throw new UserFriendlyException(
                    "Không thể xóa VIP đang có cấu hình giảm giá.");

            await _vipRepository.DeleteAsync(id);
        }

        // =====================================================
        // DANH SÁCH CẤU HÌNH VIP
        // =====================================================
        public async Task<List<CauHinhVipDto>> LayCauHinhVip(int vipId)
        {
            return await _cauHinhVipRepository
                .GetAll()
                .Where(x => x.VipId == vipId)
                .OrderByDescending(x => x.TuNgay)
                .Select(x => new CauHinhVipDto
                {
                    Id = x.Id,
                    VipId = x.VipId,
                    PhanTramGiam = x.PhanTramGiam,
                    TuNgay = x.TuNgay,
                    DenNgay = x.DenNgay
                })
                .ToListAsync();
        }

        // =====================================================
        // THÊM CẤU HÌNH VIP
        // =====================================================
        public async Task<int> ThemCauHinhVip(ThemCauHinhVipDto input)
        {
            if (input == null)
                throw new UserFriendlyException("Vui lòng nhập đầy đủ thông tin chính sách.");

            if (input.VipId <= 0)
                throw new UserFriendlyException("Vui lòng chọn cấp VIP.");

            if (input.TuNgay == default)
                throw new UserFriendlyException("Vui lòng chọn ngày bắt đầu.");

            if (input.PhanTramGiam < 0 || input.PhanTramGiam > 100)
                throw new UserFriendlyException("Phần trăm giảm phải từ 0 đến 100.");

            var tuNgay = input.TuNgay.Date;
            var denNgay = input.DenNgay?.Date;

            if (denNgay.HasValue && denNgay.Value < tuNgay)
                throw new UserFriendlyException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            var vip = await _vipRepository.FirstOrDefaultAsync(input.VipId);

            if (vip == null)
                throw new UserFriendlyException("Không tìm thấy cấp VIP.");

            var danhSachCauHinh = await _cauHinhVipRepository
                .GetAll()
                .Where(x => x.VipId == input.VipId)
                .OrderBy(x => x.TuNgay)
                .ToListAsync();

            // =====================================================
            // KHÔNG CHO TRÙNG NGÀY BẮT ĐẦU
            // =====================================================
            if (danhSachCauHinh.Any(x => x.TuNgay.Date == tuNgay))
                throw new UserFriendlyException(
                    "Đã tồn tại chính sách bắt đầu từ ngày này. Vui lòng chọn ngày khác.");

            // =====================================================
            // TÌM CHÍNH SÁCH ĐANG ÁP DỤNG TRƯỚC NGÀY MỚI
            // =====================================================
            var cauHinhTruocDo = danhSachCauHinh
                .Where(x => x.TuNgay.Date < tuNgay)
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefault();

            if (cauHinhTruocDo != null)
            {
                // Chính sách trước đó không giới hạn
                // => tự động đóng vào ngày trước ngày bắt đầu mới.
                if (!cauHinhTruocDo.DenNgay.HasValue)
                {
                    cauHinhTruocDo.DenNgay = tuNgay.AddDays(-1);

                    await _cauHinhVipRepository.UpdateAsync(cauHinhTruocDo);
                }
                else if (cauHinhTruocDo.DenNgay.Value.Date >= tuNgay)
                {
                    throw new UserFriendlyException(
                        "Ngày bắt đầu đang nằm trong khoảng thời gian của chính sách VIP trước đó.");
                }
            }

            // =====================================================
            // TÌM CHÍNH SÁCH PHÍA SAU
            // =====================================================
            var cauHinhSauDo = danhSachCauHinh
                .Where(x => x.TuNgay.Date > tuNgay)
                .OrderBy(x => x.TuNgay)
                .FirstOrDefault();

            if (cauHinhSauDo != null)
            {
                // Nếu người dùng không nhập ngày kết thúc,
                // tự động kết thúc trước ngày bắt đầu của chính sách sau.
                if (!denNgay.HasValue)
                {
                    denNgay = cauHinhSauDo.TuNgay.Date.AddDays(-1);
                }
                else if (denNgay.Value.Date >= cauHinhSauDo.TuNgay.Date)
                {
                    throw new UserFriendlyException(
                        "Ngày kết thúc không được trùng hoặc vượt quá chính sách VIP tiếp theo.");
                }
            }

            // =====================================================
            // KIỂM TRA TRÙNG KHOẢNG THỜI GIAN
            // =====================================================
            // Không dùng lại chính sách trước đó vừa được tự động đóng.
            var biTrung = danhSachCauHinh
                .Where(x => x.Id != (cauHinhTruocDo?.Id ?? 0))
                .Any(x =>
                    tuNgay <= (x.DenNgay ?? DateTime.MaxValue).Date &&
                    (denNgay ?? DateTime.MaxValue).Date >= x.TuNgay.Date);

            if (biTrung)
                throw new UserFriendlyException(
                    "Khoảng thời gian này bị trùng với chính sách VIP khác.");

            // =====================================================
            // TẠO CHÍNH SÁCH MỚI
            // =====================================================
            var cauHinh = new CauHinhVip
            {
                TenantId = AbpSession.GetTenantId(),
                VipId = input.VipId,
                PhanTramGiam = input.PhanTramGiam,
                TuNgay = tuNgay,
                DenNgay = denNgay
            };

            return await _cauHinhVipRepository.InsertAndGetIdAsync(cauHinh);
        }
        // =====================================================
        // SỬA CẤU HÌNH VIP
        // =====================================================
        public async Task SuaCauHinhVip(SuaCauHinhVipDto input)
        {
            if (input == null || input.Id <= 0)
                throw new UserFriendlyException(
                    "Thông tin cấu hình VIP không hợp lệ.");

            if (input.TuNgay == default)
                throw new UserFriendlyException(
                    "Vui lòng chọn ngày bắt đầu.");

            if (input.PhanTramGiam < 0 || input.PhanTramGiam > 100)
                throw new UserFriendlyException(
                    "Phần trăm giảm phải từ 0 đến 100.");

            var tuNgay = input.TuNgay.Date;
            var denNgay = input.DenNgay?.Date;

            if (denNgay.HasValue && denNgay.Value < tuNgay)
                throw new UserFriendlyException(
                    "Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            var cauHinh = await _cauHinhVipRepository
                .FirstOrDefaultAsync(input.Id);

            if (cauHinh == null)
                throw new UserFriendlyException(
                    "Không tìm thấy cấu hình VIP.");

            var vip = await _vipRepository
                .FirstOrDefaultAsync(input.VipId);

            if (vip == null)
                throw new UserFriendlyException(
                    "Không tìm thấy cấp VIP.");

            var danhSachCauHinh = await _cauHinhVipRepository
                .GetAll()
                .Where(x =>
                    x.VipId == input.VipId &&
                    x.Id != input.Id)
                .OrderBy(x => x.TuNgay)
                .ToListAsync();

            // =====================================================
            // TÌM CHÍNH SÁCH TRƯỚC ĐÓ
            // =====================================================
            var cauHinhTruocDo = danhSachCauHinh
                .Where(x =>
                    x.TuNgay.Date < tuNgay &&
                    (!x.DenNgay.HasValue ||
                     x.DenNgay.Value.Date >= tuNgay))
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefault();

            if (cauHinhTruocDo != null)
            {
                if (!cauHinhTruocDo.DenNgay.HasValue)
                {
                    cauHinhTruocDo.DenNgay = tuNgay.AddDays(-1);

                    await _cauHinhVipRepository
                        .UpdateAsync(cauHinhTruocDo);
                }
                else
                {
                    throw new UserFriendlyException(
                        "Ngày bắt đầu đang nằm trong khoảng thời gian của chính sách VIP trước đó.");
                }
            }

            // =====================================================
            // KIỂM TRA TRÙNG VỚI CHÍNH SÁCH KHÁC
            // =====================================================
            var biTrung = danhSachCauHinh.Any(x =>
                tuNgay <= (x.DenNgay ?? DateTime.MaxValue).Date &&
                (denNgay ?? DateTime.MaxValue).Date >= x.TuNgay.Date);

            if (biTrung)
                throw new UserFriendlyException(
                    "Khoảng thời gian này bị trùng với chính sách VIP khác.");

            // =====================================================
            // CẬP NHẬT
            // =====================================================
            cauHinh.VipId = input.VipId;
            cauHinh.PhanTramGiam = input.PhanTramGiam;
            cauHinh.TuNgay = tuNgay;
            cauHinh.DenNgay = denNgay;

            await _cauHinhVipRepository
                .UpdateAsync(cauHinh);
        }

        // =====================================================
        // XÓA CẤU HÌNH VIP
        // =====================================================
        public async Task XoaCauHinhVip(int id)
        {
            var cauHinh = await _cauHinhVipRepository
                .FirstOrDefaultAsync(id);

            if (cauHinh == null)
                throw new UserFriendlyException(
                    "Không tìm thấy cấu hình VIP.");

            await _cauHinhVipRepository
                .DeleteAsync(id);
        }

        // =====================================================
        // LẤY % GIẢM THEO CẤP VIP + NGÀY
        // =====================================================
        public async Task<decimal> LayPhanTramGiam(
            int capVip,
            DateTime ngay)
        {
            var vip = await _vipRepository
                .FirstOrDefaultAsync(x => x.CapVip == capVip);

            if (vip == null)
                return 0;

            var cauHinh = await _cauHinhVipRepository
                .GetAll()
                .Where(x =>
                    x.VipId == vip.Id &&
                    x.TuNgay <= ngay.Date &&
                    (!x.DenNgay.HasValue ||
                     x.DenNgay.Value >= ngay.Date))
                .OrderByDescending(x => x.TuNgay)
                .FirstOrDefaultAsync();

            return cauHinh?.PhanTramGiam ?? 0;
        }
    }
}