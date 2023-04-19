using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Profiles
{
    public class InventoryItemProfile : Profile
    {
        public InventoryItemProfile()
        {
            CreateMap<InventoryItem, InventoryItemReadDto>();
            CreateMap<InventoryItem, StockModel>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.ArticleId));

            CreateMap<ArticleCreateMessage, InventoryItem>()
                .ForMember(dest => dest.ArticleId,
                            opts => opts.MapFrom(src => src.Id));

            CreateMap<ArticleUpdateMessage, InventoryItemUpdate>();
            CreateMap<InventoryItemUpdate, InventoryItem>();
        }       
    }
}
