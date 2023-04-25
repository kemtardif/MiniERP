using AutoMapper;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.MessageBus.Messages;
using MiniERP.SalesOrderService.MessageBus.Sender.Contracts;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Behaviors.CreateBehavior
{
    public class MessagingBehavior : IPipelineBehavior<CreateCommand, Result<SOReadDTO>>
    {
        private readonly IRabbitMQClient _rabbitMQClient;
        private readonly IMapper _mapper;

        public MessagingBehavior(IRabbitMQClient rabbitMQClient,
                                 IMapper mapper)
        {
            _rabbitMQClient = rabbitMQClient;
            _mapper = mapper;
        }
        public async Task<Result<SOReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<SOReadDTO>> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if(response.IsSuccess)
            {
                MovementOpen mo = _mapper.Map<MovementOpen>(response.Value);

                _rabbitMQClient.Publish(mo);
            }
            return response;
        }
    }
}
