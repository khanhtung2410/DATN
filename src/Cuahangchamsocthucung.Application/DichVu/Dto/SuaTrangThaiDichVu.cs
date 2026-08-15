using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.DichVu.Dto
{
    public class SuaTrangThaiDichVuDto :EntityDto<int>
    {
        public bool Trangthai { get; set; }
    }
}
