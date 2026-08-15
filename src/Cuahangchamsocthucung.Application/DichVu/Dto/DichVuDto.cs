using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class DichVuDto 
    {
        public int Id { get; set; }
        public string Tendichvu { get; set; }

        public string Mota { get; set; }

        public bool Trangthai { get; set; }
        public List<BangGiaDto> BangGias { get; set; }
    }
}
