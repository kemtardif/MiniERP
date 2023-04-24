using AutoMapper;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class CreateHandler : IRequestHandler<CreateCommand, Result<SOReadDTO>>
    {
        private readonly ISalesOrderRepository _repository;
        private readonly IMapper _mapper;

        public CreateHandler(ISalesOrderRepository repository,
                             IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public Task<Result<SOReadDTO>> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var so = _mapper.Map<SalesOrder>(request.SalesOrder);

            so.Status = SalesOrderStatus.Open;

            _repository.AddSalesOrder(so);

            _repository.SaveChanges();

            var dto = _mapper.Map<SOReadDTO>(so);

            return Task.FromResult(Result<SOReadDTO>.Success(dto));
        }
    }
}
