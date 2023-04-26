using AutoMapper;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.MessageBus.Messages;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class CreateHandler : HandlerBase, IRequestHandler<CreateCommand, Result<SOReadDTO>>
    {
        public CreateHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {

        }
        public Task<Result<SOReadDTO>> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var so = _mapper.Map<SalesOrder>(request.SalesOrder);

            so.Status = SalesOrderStatus.Open;

            _repository.AddSalesOrder(so);

            _repository.SaveChanges();

            var dto = _mapper.Map<SOReadDTO>(so);

            var message = _mapper.Map<OrderCreated>(dto);
            message.TransactionId = request.TransactionId; 

            return Task.FromResult(Result<SOReadDTO>.Success(dto, message));
        }
    }
}
