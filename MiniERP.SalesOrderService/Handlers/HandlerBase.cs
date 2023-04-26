using AutoMapper;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class HandlerBase
    {
        protected readonly IRepository _repository;
        protected readonly IMapper _mapper;

        protected HandlerBase(IRepository repository,
                             IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        protected IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(SalesOrder)] = new string[] { $"Sales Order not found : ID = {id}" }
            };
        }
    }
}
