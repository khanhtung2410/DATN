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
        public CuahangchamsocthucungDbContext(DbContextOptions<CuahangchamsocthucungDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasIndex(x => x.SDT)
                      .IsUnique();
            });
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasIndex(x => x.SDT)
                      .IsUnique();
            });
            modelBuilder.Entity<LichChamSoc>()
        .HasOne(x => x.BangGia)
        .WithMany()
        .HasForeignKey(x => x.BangGiaId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BangGia>()
    .HasOne(x => x.DichVu)
    .WithMany(d => d.BangGias)
    .HasForeignKey(x => x.DichVuId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThuCung>()
    .HasOne(x => x.KhachHang)
    .WithMany()
    .HasForeignKey(x => x.KhachHangId)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}