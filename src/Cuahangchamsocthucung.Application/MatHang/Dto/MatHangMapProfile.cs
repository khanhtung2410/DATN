using AutoMapper;
using Cuahangchamsocthucung.MatHang.Dto;
using EntityMatHang = Cuahangchamsocthucung.Entities.MatHang;

namespace Cuahangchamsocthucung.MatHang.Dto
{
    public class MatHangMapProfile : Profile
    {
        public MatHangMapProfile()
        {
            CreateMap<EntityMatHang, MatHangDto>();
            CreateMap<ThemMatHangDto, EntityMatHang>();
            CreateMap<SuaMatHangDto, EntityMatHang>();
        }
    }
}
