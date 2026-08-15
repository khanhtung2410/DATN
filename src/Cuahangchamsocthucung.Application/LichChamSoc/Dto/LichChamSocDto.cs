using Cuahangchamsocthucung.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.LichChamSoc.Dto
{
    public class LichChamSocDto
    {
        public int Id { get; set; }
        public int DichVuId { get; set; }
        public int BangGiaId { get; set; }
        public int? NhanVienId { get; set; }
        public int KhachHangId { get; set; }
        public DateTime ThoiGian { get; set; }
        public string TenDichVu { get; set; }
        public TrangThaiLichChamSoc TrangThai { get; set; }
    }
}
