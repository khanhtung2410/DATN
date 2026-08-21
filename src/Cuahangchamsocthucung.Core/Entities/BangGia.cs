using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchamsocthucung.Entities
{
    public class BangGia : AuditedEntity<int>, IValidatableObject
    {
        [Required] 
        public int DichVuId { get; set; }
        [Required, MaxLength(50)] 
        public string Loaithucung { get; set; }
        [MaxLength(50)] 
        public string LoaiPhong { get; set; }
        [Required, Range(0, int.MaxValue, ErrorMessage = "Khoảng cân nặng không hợp lệ.")] 
        public int Cannangtu { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Cannangden { get; set; }
        [Required] 
        public bool Loailong { get; set; }
        [Required, Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Giá dịch vụ không hợp lệ.")] 
        public decimal Giadv { get; set; }
        [Required, Range(1, 1440, ErrorMessage = "Thời gian phải từ 1 đến 1440 phút.")]
        public int ThoiGianPhut { get; set; }

        [ForeignKey(nameof(DichVuId))]
        public virtual DichVu DichVu { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Cannangtu >= Cannangden)
                yield return new ValidationResult("Khoảng cân nặng không hợp lệ.", new[] { nameof(Cannangtu), nameof(Cannangden) });
        }

        public BangGia() { }

        public BangGia(int dichvuId, decimal giadv, string loaithucung, int cannangtu, int cannangden, bool loailong, int thoiGianPhut, string loaiPhong = null)
        {
            DichVuId = dichvuId;
            Giadv = giadv;
            Loaithucung = loaithucung;
            Cannangtu = cannangtu;
            Cannangden = cannangden;
            Loailong = loailong;
            ThoiGianPhut = thoiGianPhut;
            LoaiPhong = loaiPhong;
        }
    }
}