using AutoMapper;
using Grpc.Core;
using MiniERP.SalesOrderService.Caching;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;
using StackExchange.Redis;

namespace MiniERP.SalesOrderService.Data
{
    public class RedisCache : ICache
    {
        private readonly IDatabase _database;
        private readonly IDataClient _dataClient;
        private readonly ILogger<RedisCache> _logger;
        private readonly IMapper _mapper;

        public RedisCache(IDatabase database,
                          IDataClient dataclient,
                          ILogger<RedisCache> logger,
                          IMapper mapper)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _dataClient = dataclient ?? throw new ArgumentNullException(nameof(dataclient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public StockModel? GetCachedStockModel(int id)
        {
            StockModel? stockModel = GetFromCache(id);

            if(stockModel is not null)
            {
                _logger.LogInformation("---> REDIS Cache HIT for ID={id}", id);
                return stockModel;
            }
            _logger.LogInformation("---> REDIS Cache MISS for ID={id}. Fetchinf via GRPC...", id);

            stockModel = GetFromService(id);

            _logger.LogInformation("---> REDIS DataClient {result} for ID={id}", 
                                    stockModel is null ? "NOT FOUND" : "FOUND",
                                    id);

            return stockModel;
        }
        private StockModel? GetFromService(int id)
        {
            try
            {
                StockResponse response = _dataClient.GetAvailableStock(id);

                if (response.IsFound)
                {
                    return response.Item;

                }
            } 
            catch(RpcException rpcEx)
            {
                _logger.LogError(rpcEx,"----> REDIS RPC Exception for ID={id}", id);
            }
            return null;
        }

        private StockModel? GetFromCache(int id)
        {
            try
            {
                //RedisServerException: WRONGTYPE Operation against a key holding the wrong kind of value
                CachedStock? cachedStock = _database.GetRecord<CachedStock>(id.ToString());

                if(cachedStock is not null)
                {
                    return _mapper.Map<StockModel>(cachedStock);
                }
            } 
            catch(RedisTimeoutException timeoutEx)
            {
                _logger.LogError(timeoutEx, "---> REDIS Timeout for ID={id}", id);
            }
            return null;
        }
    }
}
