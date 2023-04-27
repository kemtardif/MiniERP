using AutoMapper;

namespace MiniERP.InventoryService.Handlers
{
    public class HandlerBase
    {
        protected readonly IMapper _mapper;

        public HandlerBase( IMapper mapper)
        {
            _mapper = mapper;
        }

        protected IDictionary<string, string[]> GetNotFoundResult<T>(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(T)] = new string[] { $"Item not found : ID = {id}" }
            };
        }
    }
}
