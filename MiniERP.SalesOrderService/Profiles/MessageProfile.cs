using AutoMapper;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.MessageBus.Messages;

namespace MiniERP.SalesOrderService.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<SODetailReadDTO, OrderItem>();

            CreateMap<SOReadDTO, OrderCreated>()
                .ForMember(dest => dest.OrderType,
                        opts => opts.MapFrom(x => (int)RelatedOrderType.SO))
                .ForMember(dest => dest.OrderId,
                        opts => opts.MapFrom(x => x.Id))
                .ForMember(dest => dest.Items,
                        opts => opts.MapFrom(x => x.Details));
        }
    }
}
