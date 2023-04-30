using AutoMapper;
using MiniERP.InventoryService.Models.Views;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Profiles
{
    public class InventoryMovementProfile : Profile
    {
        public InventoryMovementProfile()
        {
            CreateMap<AvailableInventoryView, StockModel>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.ArticleId));
        }
    }
}
