using AutoMapper;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.MessageBus.Messages;

namespace MiniERP.SalesOrderService.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<SODetailReadDTO, MovementOpenItem>()
                .ForMember(dest => dest.MovementType,
                        opts => opts.MapFrom(x => MovementType.Out));

            CreateMap<SOReadDTO, MovementOpen>()
                .ForMember(dest => dest.RelatedOrderType,
                        opts => opts.MapFrom(x => RelatedOrderType.SO))
                .ForMember(dest => dest.RelatedOrderId,
                        opts => opts.MapFrom(x => x.Id))
                .ForMember(dest => dest.MovementItems,
                        opts => opts.MapFrom(x => x.Details));
        }
    }
}
