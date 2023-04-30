using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Extensions;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;
using Polly;
using StackExchange.Redis;

namespace MiniERP.InventoryService.Grpc
{
    public class GrpcInventoryService : Protos.GrpcInventoryService.GrpcInventoryServiceBase
    {
        private const string CallLogFormat = "GRPC  {method} : {id} : {date}";
        private const string ExceptionLogFormat = "GRPC error {method} : {date}";
        private const string TimeoutExceptionLogFormat = "---> GRPC Cache Timeout for ID={id}";

        private readonly ILogger<GrpcInventoryService> _logger;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IStockSourcing _source;
        private readonly ISyncPolicy _cachePolicy;

        public GrpcInventoryService(ILogger<GrpcInventoryService> logger,
                                    IMapper mapper,
                                    IDistributedCache cache,
                                    IStockSourcing source,
                                    ISyncPolicy cachePolicy)
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _source = source;
            _cachePolicy = cachePolicy;
        }

        public override Task<StockResponse> GetInventory(StockRequest request, ServerCallContext context)
        {
            try
            {
                //Get from inventory => This ia a backup service
                Stock? stock = _source.GetMinForecastById(request.ArticleId);

                if (stock is null)
                {
                    return Task.FromResult(NotFoundResponse(request.ArticleId));
                }

                SetCache(stock);

                StockModel sm = _mapper.Map<StockModel>(stock);

                _logger.LogInformation(CallLogFormat,
                                    nameof(GetInventory),
                                    sm.Id,
                                    DateTime.UtcNow);

                return Task.FromResult(new StockResponse() { ArticleId = request.ArticleId, IsFound = true, Item = sm });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogFormat,
                                    nameof(GetInventory),
                                    DateTime.UtcNow);
                return Task.FromResult(NotFoundResponse(request.ArticleId));
            }
        }

        private void SetCache(Stock stock)
        {
            //Catch timeout error if cache is down. We don't want to stop Service.
            try
            {
                var pollyContext = new Context().WithLogger<ILogger<GrpcInventoryService>>(_logger);

                _cachePolicy.Execute((cntx) =>
                   _cache.SetRecord(stock.ArticleId.ToString(), stock), pollyContext);
            }
            catch (RedisTimeoutException timeoutEx)
            {
                _logger.LogError(timeoutEx, TimeoutExceptionLogFormat, stock.ArticleId);
            }
        }

        private StockResponse NotFoundResponse(int id)
        {
            return new StockResponse() { ArticleId = id, IsFound = false };
        }
    }
}
