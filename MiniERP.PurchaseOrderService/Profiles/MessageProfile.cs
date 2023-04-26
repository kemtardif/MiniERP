using AutoMapper;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;

namespace MiniERP.PurchaseOrderService.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<PODetailReadDTO, OrderItem>()
                .ForMember(dest => dest.ArticleId,
                        opts => opts.MapFrom(x => x.Productd));

            CreateMap<POReadDTO, OrderCreated>()
                .ForMember(dest => dest.OrderType,
                        opts => opts.MapFrom(x => (int)RelatedOrderType.PO))
                .ForMember(dest => dest.OrderId,
                        opts => opts.MapFrom(x => x.Id))
                .ForMember(dest => dest.Items,
                        opts => opts.MapFrom(x => x.Details));
        }
    }
}
