using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using MiniERP.InventoryService.Caching;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;


namespace MiniERP.InventoryService.Grpc
{
    public class GrpcInventoryService : Protos.GrpcInventoryService.GrpcInventoryServiceBase
    {
        private readonly ILogger<GrpcInventoryService> _logger;
        private readonly IMapper _mapper;
        private readonly ICache _cache;

        public GrpcInventoryService(ILogger<GrpcInventoryService> logger,
                                    IMapper mapper,
                                    ICache cache)
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
        }

        public override Task<StockResponse> GetInventory(StockRequest request, ServerCallContext context)
        {
            try
            {
                AvailableInventoryView? inventory = _cache.GetById(request.ArticleId);

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

    }
}
