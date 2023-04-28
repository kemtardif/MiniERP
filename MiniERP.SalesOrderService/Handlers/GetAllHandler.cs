using AutoMapper;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Queries;

namespace MiniERP.SalesOrderService.Handlers
{
    public class GetAllHandler : HandlerBase<GetAllQuery, Result<IEnumerable<SOReadDTO>>>
    {
        public GetAllHandler(IRepository repository, IMapper mapper) : base(repository, mapper){ }

        public override Task<Result<IEnumerable<SOReadDTO>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<SalesOrder> sos = _repository.GetAllSalesOrders();

            var dtos = _mapper.Map<IEnumerable<SOReadDTO>>(sos);

            return Task.FromResult(Result<IEnumerable<SOReadDTO>>.Success(dtos));
        }
    }
}
