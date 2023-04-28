using AutoMapper;
using MediatR;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Handlers
{
    public abstract class HandlerBase<TResponse, TResult> : IRequestHandler<TResponse, TResult>
                                            where TResponse : IRequest<TResult>
    {
        protected readonly IRepository _repository;
        protected readonly IMapper _mapper;

        protected HandlerBase(IRepository repository,
                             IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public abstract Task<TResult> Handle(TResponse request, CancellationToken cancellationToken);

        protected IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(PurchaseOrder)] = new string[] { $"Purchase Order not found : ID = {id}" }
            };
        }
    }
}
