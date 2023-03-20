using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Profiles
{
    public class ProductStockProfile : Profile
    {
        public ProductStockProfile()
        {
            CreateMap<Stock, StockReadDto>();
            CreateMap<ArticleResponse, Stock>()
                .ForSourceMember(x => x.EventName, opts => opts.DoNotValidate())
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id));
        }
    }
}
