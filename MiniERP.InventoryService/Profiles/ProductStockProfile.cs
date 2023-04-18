using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Messages;
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
            //CreateMap<InventoryItem, StockModel>()
            //    .ForMember(dest => dest.Id,
            //                opt => opt.MapFrom(src => src.ProductId))
            //    .ForMember(dest => dest.Quantity,
            //                opt => opt.MapFrom(src => src.Stock.Quantity))
            //    .ForMember(dest => dest.Status,
            //                opt => opt.MapFrom(src => src.Status));
            CreateMap<OpenStockModel, StockMovement>();
            CreateMap<Stock, StockModel>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.InventoryId));

            CreateMap<ArticleMessage, InventoryItem>()
                .ForSourceMember(x => x.EventName, opts => opts.DoNotValidate())
                .ForMember(dest => dest.ArticleId,
                            opts => opts.MapFrom(src => src.Id));

        }       
    }
}
