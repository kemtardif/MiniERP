using AutoMapper;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Profiles
{
    public class InventoryViewProfile : Profile
    {
        public InventoryViewProfile()
        {
            CreateMap<AvailableInventoryView, StockModel>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.ArticleId));

            CreateMap<PendingInventoryView, StockModel>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.ArticleId));
        }
    }
}
