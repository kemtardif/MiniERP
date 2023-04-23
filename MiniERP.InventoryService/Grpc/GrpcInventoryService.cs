using AutoMapper;
using Grpc.Core;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Grpc
{
    public class GrpcInventoryService : Protos.GrpcInventoryService.GrpcInventoryServiceBase
    {
        private readonly ILogger<GrpcInventoryService> _logger;
        private readonly IMapper _mapper;
        private readonly IStockCache _cache;

        public GrpcInventoryService(ILogger<GrpcInventoryService> logger,
                                        IMapper mapper,
                                        IStockCache cache)
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
        }

        public override Task<StockResponse> GetInventory(StockRequest request, ServerCallContext context)
        {
            try
            {
                AvailableInventoryView? inventory = _cache.GetAvailableByArticleId(request.ArticleId);

                if (inventory is null)
                {
                    return Task.FromResult(new StockResponse() { ArticleId = request.ArticleId, IsFound = false });
                }

                StockModel sm = _mapper.Map<StockModel>(inventory);

                _logger.LogInformation("GRPC  {method} : {id} : {date}",
                                    nameof(GetInventory),
                                    sm.Id,
                                    DateTime.UtcNow);

                return Task.FromResult(new StockResponse() { ArticleId = request.ArticleId, IsFound = true, Item = sm });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(GetInventory),
                                    DateTime.UtcNow);
                return Task.FromResult(new StockResponse() { IsFound = false });
            }
        }


        public override async Task GetInventoryStream(IAsyncStreamReader<StockRequest> callStream, IServerStreamWriter<StockResponse> responseStream, ServerCallContext context)
        {
            try
            {
                while (await callStream.MoveNext())
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    AvailableInventoryView? inventory = _cache.GetAvailableByArticleId(callStream.Current.ArticleId);

                    if (inventory is null)
                    {
                        await responseStream.WriteAsync(new StockResponse() { ArticleId = callStream.Current.ArticleId, IsFound = false });
                        continue;
                    }

                    StockModel stock = _mapper.Map<StockModel>(inventory);

                    await responseStream.WriteAsync(new StockResponse() { ArticleId = callStream.Current.ArticleId, IsFound = true, Item = stock });

                    _logger.LogInformation("GRPC  {method} : {id} : {date}",
                                    nameof(GetInventoryStream),
                                    stock.Id,
                                    DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(GetInventoryStream),
                                    DateTime.UtcNow);

            }
        }

        public override Task<StockResponse> GetForecastInventory(StockRequest request, ServerCallContext context)
        {
            try
            {
                PendingInventoryView? inventory = _cache.GetPendingByArticleId(request.ArticleId);

                if (inventory is null)
                {
                    return Task.FromResult(new StockResponse() { ArticleId = request.ArticleId, IsFound = false });
                }

                StockModel sm = _mapper.Map<StockModel>(inventory);

                _logger.LogInformation("GRPC  {method} : {id} : {date}",
                                    nameof(GetForecastInventory),
                                    sm.Id,
                                    DateTime.UtcNow);

                return Task.FromResult(new StockResponse() { ArticleId = request.ArticleId, IsFound = true, Item = sm });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(GetForecastInventory),
                                    DateTime.UtcNow);
                return Task.FromResult(new StockResponse() { IsFound = false });
            }
        }
        public override async Task GetForecastInventoryStream(IAsyncStreamReader<StockRequest> callStream, IServerStreamWriter<StockResponse> responseStream, ServerCallContext context)
        {
            try
            {
                while (await callStream.MoveNext())
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    PendingInventoryView? inventory = _cache.GetPendingByArticleId(callStream.Current.ArticleId);

                    if (inventory is null)
                    {
                        await responseStream.WriteAsync(new StockResponse() { ArticleId = callStream.Current.ArticleId, IsFound = false });
                        continue;
                    }

                    StockModel stock = _mapper.Map<StockModel>(inventory);

                    await responseStream.WriteAsync(new StockResponse() { ArticleId = callStream.Current.ArticleId, IsFound = true, Item = stock });

                    _logger.LogInformation("GRPC  {method} : {id} : {date}",
                                    nameof(GetForecastInventoryStream),
                                    stock.Id,
                                    DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(GetForecastInventoryStream),
                                    DateTime.UtcNow);

            }
        }
    }
}
