using AutoMapper;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Profiles
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
        {
            CreateMap<Unit, UnitReadDto>();
            CreateMap<UnitWriteDto, Unit>();
        }
    }
}
