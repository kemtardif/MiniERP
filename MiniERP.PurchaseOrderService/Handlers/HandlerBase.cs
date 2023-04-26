using AutoMapper;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Handlers
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
                [nameof(PurchaseOrder)] = new string[] { $"Purchase Order not found : ID = {id}" }
            };
        }
    }
}
