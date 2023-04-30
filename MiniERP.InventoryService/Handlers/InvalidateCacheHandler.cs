using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Commands;

namespace MiniERP.InventoryService.Handlers
{
    public class InvalidateCacheHandler : IRequestHandler<InvalidateCacheCommand>
    {
        private const string InvalidationLogFormat = "------> REDIS : Cache INVALIDATION for {id}";

        private readonly IDistributedCache _cache;
        private readonly ILogger<InvalidateCacheHandler> _logger;

        public InvalidateCacheHandler(IDistributedCache cache,
                                      ILogger<InvalidateCacheHandler> logger) 
        {
            _cache = cache;
            _logger = logger;
        }

        Task IRequestHandler<InvalidateCacheCommand>.Handle(InvalidateCacheCommand request, CancellationToken cancellationToken)
        {
            _cache.Remove(request.Id.ToString());

            _logger.LogInformation(InvalidationLogFormat, request.Id);

            return Task.CompletedTask;
        }
    }
}
