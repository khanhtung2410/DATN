using Abp.Zero.EntityFrameworkCore;
using Cuahangchamsocthucung.Authorization.Roles;
using Cuahangchamsocthucung.Authorization.Users;
using Cuahangchamsocthucung.Entities;
using Cuahangchamsocthucung.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Cuahangchamsocthucung.EntityFrameworkCore
{
    public class CuahangchamsocthucungDbContext : AbpZeroDbContext<Tenant, Role, User, CuahangchamsocthucungDbContext>
    {
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<BangGia> BangGias { get; set; }
        public DbSet<MatHang> MatHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<LichChamSoc> LichChamSocs { get; set; }
        public DbSet<ThuCung> ThuCungs { get; set; }
        public DbSet<Vip> Vips { get; set; }
        public DbSet<CauHinhVip> CauHinhVips { get; set; }

        public CuahangchamsocthucungDbContext(
            DbContextOptions<CuahangchamsocthucungDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // NHÂN VIÊN
            // =========================
            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasIndex(x => x.SDT).IsUnique();
            });

            // =========================
            // KHÁCH HÀNG -> VIP
            // =========================
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasIndex(x => x.SDT).IsUnique();

                entity.HasOne(x => x.Vip)
                    .WithMany(x => x.KhachHangs)
                    .HasForeignKey(x => x.VipId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // CẤU HÌNH VIP
            // Vip 1 -> nhiều cấu hình theo thời gian
            // =========================
            modelBuilder.Entity<CauHinhVip>(entity =>
            {
                entity.HasOne(x => x.Vip)
                    .WithMany(x => x.CauHinhVips)
                    .HasForeignKey(x => x.VipId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.PhanTramGiam)
                    .HasPrecision(5, 2);
            });

            // =========================
            // LỊCH CHĂM SÓC -> BẢNG GIÁ
            // =========================
            modelBuilder.Entity<LichChamSoc>()
                .HasOne(x => x.BangGia)
                .WithMany()
                .HasForeignKey(x => x.BangGiaId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // BẢNG GIÁ -> DỊCH VỤ
            // =========================
            modelBuilder.Entity<BangGia>()
                .HasOne(x => x.DichVu)
                .WithMany(d => d.BangGias)
                .HasForeignKey(x => x.DichVuId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // THÚ CƯNG -> KHÁCH HÀNG
            // =========================
            modelBuilder.Entity<ThuCung>()
                .HasOne(x => x.KhachHang)
                .WithMany()
                .HasForeignKey(x => x.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // HÓA ĐƠN -> LỊCH CHĂM SÓC
            // =========================
            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.LichChamSoc)
                .WithMany()
                .HasForeignKey(x => x.LichChamSocId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // HÓA ĐƠN -> KHÁCH HÀNG
            // =========================
            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.KhachHang)
                .WithMany()
                .HasForeignKey(x => x.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // HÓA ĐƠN -> NHÂN VIÊN
            // =========================
            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.NhanVien)
                .WithMany()
                .HasForeignKey(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // CHI TIẾT HÓA ĐƠN -> HÓA ĐƠN
            // =========================
            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(x => x.HoaDon)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.HoaDonId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CHI TIẾT HÓA ĐƠN -> DỊCH VỤ
            // =========================
            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(x => x.DichVu)
                .WithMany()
                .HasForeignKey(x => x.DichVuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}