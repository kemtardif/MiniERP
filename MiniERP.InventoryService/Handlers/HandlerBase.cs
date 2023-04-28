using AutoMapper;
using MediatR;
using MiniERP.InventoryService.Data;

namespace MiniERP.InventoryService.Handlers
{
    public abstract class HandlerBase<TResponse, TResult> : IRequestHandler<TResponse, TResult>
                                            where TResponse : IRequest<TResult>
    {
        private const string NotFoundMessage = "Item not found : ID = {0}";

        protected readonly IRepository _repository;
        protected readonly IMapper _mapper;
        public HandlerBase(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public abstract Task<TResult> Handle(TResponse request, CancellationToken cancellationToken);

        protected IDictionary<string, string[]> GetNotFoundResult<T>(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(T)] = new string[] { string.Format(NotFoundMessage, id) }
            };
        }
    }
}
