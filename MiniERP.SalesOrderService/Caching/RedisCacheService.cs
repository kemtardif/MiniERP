using AutoMapper;
using MiniERP.SalesOrderService.Extensions;
using MiniERP.SalesOrderService.Models;
using Polly;
using StackExchange.Redis;

namespace MiniERP.SalesOrderService.Caching
{
    public class RedisCacheService : ICacheService
    {
        private const string TimeoutExceptionLogFormat = "---> REDIS Timeout for ID={id}";
        private const string CacheHitLogFormat = "---> REDIS Cache {value} for ID={id}";

        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IMapper _mapper;
        private readonly ISyncPolicy<InventoryItemCache?> _cachePolicy;

        public RedisCacheService(IDatabase database,
                                 ILogger<RedisCacheService> logger,
                                 IMapper mapper,
                                 ISyncPolicy<InventoryItemCache?> cachePolicy)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cachePolicy = cachePolicy ?? throw new ArgumentNullException(nameof(cachePolicy));
        }
        public InventoryItem? GetItemById(int id)
        {
            try
            {
                var cached = _cachePolicy.Execute(
                    context => _database.GetRecord<InventoryItemCache>(id.ToString()),
                    new Context().WithLogger<ILogger<RedisCacheService>>(_logger));

                if (cached != null)
                {
                    _logger.LogInformation(CacheHitLogFormat, "HIT", id);
                    return _mapper.Map<InventoryItem>(cached);
                }
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, TimeoutExceptionLogFormat, id);
            }

            _logger.LogInformation(CacheHitLogFormat, "MISS", id);
            return null;
        }
    }
}
