using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Events;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Profiles
{
    public class ProductStockProfile : Profile
    {
        public ProductStockProfile()
        {
            CreateMap<InventoryItem, StockReadDto>()
                .ForMember(x => x.Quantity, opt => opt.MapFrom(src => src.Stock.Quantity));
            CreateMap<ArticleResponse, InventoryItem>()
                .ForSourceMember(x => x.EventName, opts => opts.DoNotValidate())
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id));
            CreateMap<InventoryItem, StockModel>()
                .ForMember(dest => dest.Id,
                            opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity,
                            opt => opt.MapFrom(src => src.Stock.Quantity))
                .ForMember(dest => dest.Status,
                            opt => opt.MapFrom(src => src.Status));
            CreateMap<StockModel, StockChange>()
                .ForMember(dest => dest.NewValue,
                            opt => opt.MapFrom(src => src.Quantity));
            CreateMap<ArticleResponse, InventoryItem>()
                .ForSourceMember(x => x.EventName, opts => opts.DoNotValidate())
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id));
        }       
    }
}
