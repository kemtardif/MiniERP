using AutoMapper;
using CommonLib.Dtos;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Profiles
{
    public class ProductStockProfile : Profile
    {
        public ProductStockProfile()
        {
            CreateMap<Stock, StockReadDto>();
            CreateMap<ArticleEventDto, Stock>()
                .ForMember(dest => dest.ProductId,
                            opt =>opt.MapFrom(src => src.Id));
        }
    }
}
