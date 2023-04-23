using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Profiles
{
    public class InventoryItemProfile : Profile
    {
        public InventoryItemProfile()
        {
            CreateMap<InventoryItem, InventoryItemReadDto>();
            

            CreateMap<ArticleCreate, InventoryItem>()
                .ForMember(dest => dest.ArticleId,
                            opts => opts.MapFrom(src => src.Id));

            CreateMap<ArticleUpdate, InventoryItem>()
                .ForMember(dest => dest.Id,
                            opts => opts.Ignore());
        }       
    }
}
