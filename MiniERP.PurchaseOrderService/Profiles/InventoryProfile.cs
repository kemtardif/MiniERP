using AutoMapper;
using MiniERP.PurchaseOrderService.Grpc.Protos;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Profiles
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
