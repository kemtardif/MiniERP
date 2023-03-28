using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Profiles
{
    public class ProductStockProfile : Profile
    {
        public ProductStockProfile()
        {
            CreateMap<Stock, StockReadDto>();
            CreateMap<StockUpdateDto, Stock>();
            CreateMap<ArticleResponse, Stock>()
                .ForSourceMember(x => x.EventName, opts => opts.DoNotValidate())
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcInventoryItemModel, Stock>()
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcInventoryItemModel, StockUpdateDto>()
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id));
        }
    }
}
