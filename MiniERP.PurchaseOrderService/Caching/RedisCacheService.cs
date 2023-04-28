using AutoMapper;
using MiniERP.PurchaseOrderService.Extensions;
using MiniERP.PurchaseOrderService.Models;
using Polly;
using StackExchange.Redis;

namespace MiniERP.PurchaseOrderService.Caching
{
    public class RedisCacheService : ICacheService
    {
        private const string TimeoutExceptionLogFormat = "---> REDIS Timeout for ID={id}";
        private const string CacheHitLogFormat = "---> REDIS Cache {value} for ID={id}";

        private readonly ILogger<RedisCacheService> _logger;
        private readonly IDatabase _database;
        private readonly IMapper _mapper;
        private readonly ISyncPolicy<InventoryItemCache?> _redisPolicy;
        public RedisCacheService(ILogger<RedisCacheService> logger,
                                 IDatabase database,
                                 IMapper mapper,
                                 ISyncPolicy<InventoryItemCache?> redisPolicy)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _redisPolicy = redisPolicy ?? throw new ArgumentNullException(nameof(redisPolicy));
        }
        public InventoryItem? GetInventoryById(int id)
        {
            InventoryItem? item = null;
            try
            {
                var context = new Context().WithLogger<ILogger<RedisCacheService>>(_logger);

                InventoryItemCache? cached = _redisPolicy.Execute((cntx) =>
                    _database.GetRecord<InventoryItemCache>(id.ToString()), context);

                if (cached is not null)
                {
                    item = _mapper.Map<InventoryItem>(cached);
                }
            }
            catch (RedisTimeoutException timeoutEx)
            {
                _logger.LogError(timeoutEx, TimeoutExceptionLogFormat, id);
            }

            _logger.LogInformation(CacheHitLogFormat,
                            item is null ? "MISS" : "HIT",
                            id);
            return item;
        }
    }
}
