using AutoMapper;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;

namespace MiniERP.PurchaseOrderService.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<PODetailReadDTO, MovementOpenItem>()
                .ForMember(dest => dest.MovementType,
                        opts => opts.MapFrom(x => MovementType.In))
                .ForMember(dest => dest.ArticleId,
                        opts => opts.MapFrom(x => x.Productd));

            CreateMap<POReadDTO, MovementOpen>()
                .ForMember(dest => dest.RelatedOrderType,
                        opts => opts.MapFrom(x => RelatedOrderType.PO))
                .ForMember(dest => dest.RelatedOrderId,
                        opts => opts.MapFrom(x => x.Id))
                .ForMember(dest => dest.MovementItems,
                        opts => opts.MapFrom(x => x.Details));
        }
    }
}
