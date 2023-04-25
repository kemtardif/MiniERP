using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.MessageBus.Messages;
using MiniERP.PurchaseOrderService.MessageBus.Sender.Contracts;
using MiniERP.PurchaseOrderService.Models;
using System.Text.Json;

namespace MiniERP.PurchaseOrderService.Behaviors.CreateBehavior
{
    public class MessagingBehavior : IPipelineBehavior<CreateCommand, Result<POReadDTO>>
    {
        private readonly IRabbitMQClient _rabbitMQClient;
        private readonly IMapper _mapper;

        public MessagingBehavior(IRabbitMQClient rabbitMQClient,
                                 IMapper mapper)
        {
            _rabbitMQClient = rabbitMQClient ?? throw new ArgumentNullException(nameof(rabbitMQClient));
            _mapper = mapper;
        }
        public async Task<Result<POReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<POReadDTO>> next, CancellationToken cancellationToken)
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
