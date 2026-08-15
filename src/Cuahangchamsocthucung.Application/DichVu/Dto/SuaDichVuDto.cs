using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class SuaDichVuDto 
    {
        [Required]
        public int Id { get; set; }
        public string Tendichvu { get; set; }
        public string Mota { get; set; }
        public bool Trangthai { get; set; }
        public List<SuaBangGiaDto> BangGias { get; set; }
    }
    public class SuaBangGiaDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int DichVuId { get; set; }
        public string Loaithucung { get; set; }
        public bool Loailong { get; set; }
        public int Cannangtu { get; set; }
        public int Cannangden { get; set; }
        public decimal Giadv { get; set; }
    }
}
