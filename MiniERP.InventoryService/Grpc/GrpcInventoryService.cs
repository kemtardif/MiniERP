using AutoMapper;
using Grpc.Core;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;
using System.Transactions;

namespace MiniERP.InventoryService.Grpc
{
    public class GrpcInventoryService : GrpcInventory.GrpcInventoryBase
    {
        private readonly IInventoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GrpcInventoryService> _logger;

        public GrpcInventoryService(IInventoryRepository repository, 
                                    IMapper mapper,
                                    ILogger<GrpcInventoryService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        public override Task<GrpcInventoryResponse> InventoryItemsCreated(InventoryItemRequest request, ServerCallContext context)
        {
            var response = new GrpcInventoryResponse();
            int processed = 0;
            try
            {
                using var scope = new TransactionScope();

                    foreach (GrpcInventoryItemModel model in request.InventoryItems)
                    {
                        Stock stock = _mapper.Map<Stock>(model);

                        _repository.AddItem(stock);
                        processed++;
                    }

                    _repository.SaveChanges();

                    response.ItemProcessed = processed;

                _logger.LogInformation($"Inventory Items added via GRPC : {processed}");

                scope.Complete();
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while receiving GRPC call InventoryItemsCreated");

                response.ItemProcessed = 0;
            }

            return Task.FromResult(response);
        }
        public override Task<GrpcInventoryResponse> InventoryItemsUpdated(InventoryItemRequest request, ServerCallContext context)
        {
            var response = new GrpcInventoryResponse();
            int processed = 0;

            try
            {
                using var scope = new TransactionScope();

                foreach (GrpcInventoryItemModel model in request.InventoryItems)
                {
                    Stock? stock = _repository.GetItemByArticleIdFromSource(model.Id);

                    if (stock is null)
                    {
                        _repository.AddItem(_mapper.Map<Stock>(model));
                    }
                    else
                    {
                       var stockUpdateDto = _mapper.Map<StockUpdateDto>(model);

                       _mapper.Map(stockUpdateDto, stock);
                    }

                    processed++;
                }

                _repository.SaveChanges();

                response.ItemProcessed = processed;

                _logger.LogInformation("Inventory Items updated via GRPC : {processed}", processed);

                scope.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while receiving GRPC call InventoryItemsCreated");

                response.ItemProcessed = 0;
            }

            return Task.FromResult(response);
        }
    }
}
