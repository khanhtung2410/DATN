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
    public class BangGiaDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int DichvuId { get; set; }
        [Required]
        public string Loaithucung { get; set; }
        public bool Loailong { get; set; }
        public int Cannangtu { get; set; }
        public int Cannangden { get; set; }
        public decimal Giadv { get; set; }
    }
}
