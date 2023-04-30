using AutoMapper;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Profiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {

            CreateMap<StockModel, InventoryItem>();

            CreateMap<InventoryItemCache, InventoryItem>()
                .ForMember(dest => dest.Id,
                        opts => opts.MapFrom(src => src.ArticleId));
        }
    }
}
