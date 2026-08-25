using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Entities
{
    public class NhanVien : Entity<int>
    {
        [Required]
        [StringLength(100)]
        public string Hoten { get; set; }

        [Required]
        [StringLength(10)]
        public string SDT { get; set; }
        public bool Gioitinh { get; set; }
        public DateOnly? Ngaysinh { get; set; }
        public DateOnly Ngayvaolam { get; set; }

        // Lương tháng
        public decimal Luong { get; set; }
        public bool Trangthai { get; set; }
        public NhanVien()
        {
        }

        public NhanVien(
            string hoten,
            string sdt,
            bool gioitinh,
            DateOnly? ngaysinh,
            DateOnly ngayvaolam,
            decimal luong,
            bool trangthai)
        {
            Hoten = hoten;
            SDT = sdt;
            Gioitinh = gioitinh;
            Ngaysinh = ngaysinh;
            Ngayvaolam = ngayvaolam;
            Luong = luong;
            Trangthai = trangthai;
        }
    }
}
