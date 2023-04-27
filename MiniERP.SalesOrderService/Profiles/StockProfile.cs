using AutoMapper;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Profiles
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<CachedStock, StockModel>()
                .ForMember(dest => dest.Id,
                        opt => opt.MapFrom(s => s.ArticleId));
        }
    }
}
