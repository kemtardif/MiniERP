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
        private readonly IInventoryRepository _repository;
        private readonly IMovementProcessor _movementProcessor;

        public GrpcInventoryService(ILogger<GrpcInventoryService> logger,
                                        IMapper mapper,
                                        IInventoryRepository repository,
                                        IMovementProcessor movementProcessor)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _movementProcessor = movementProcessor;
        }

        public override Task<GetStockResponse> GetStock(GetStockRequest request, ServerCallContext context)
        {
            try
            {

                InventoryItem? inventory = _repository.GetItemByArticleId(request.ArticleId, true, false);

                if (inventory is null)
                {
                    return Task.FromResult(new GetStockResponse() { IsFound = false });
                }

                StockModel sm = _mapper.Map<StockModel>(inventory.Stock);

                _logger.LogInformation("GRPC  {method} : {id} {date}",
                                    nameof(GetStock),
                                    sm.Id,
                                    DateTime.UtcNow);

                return Task.FromResult(new GetStockResponse() { IsFound = true, Item = sm });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(OpenStockMovement),
                                    DateTime.UtcNow);
                return Task.FromResult(new GetStockResponse() { IsFound = false });
            }
        }

        public override Task<OpenStockResponse> OpenStockMovement(OpenStockRequest request, ServerCallContext context)
        { 

            try
            {
                foreach (OpenStockModel item in request.Items)
                {
                    InventoryItem? inventory = _repository.GetItemByArticleId(item.ArticleId, false, true);
                    if (inventory is null)
                    {
                        return Task.FromResult(new OpenStockResponse() { Success = false, Message = $"Could not find item with ID = {item.ArticleId}" });
                    }

                    StockMovement sm = _mapper.Map<StockMovement>(item);

                    if(!_movementProcessor.Open(inventory, sm, out string error))
                    {
                        return Task.FromResult(new OpenStockResponse() { Success = false, Message = error });
                    }

                    inventory.StockMovements.Add(sm);
                }

                _repository.SaveChanges();

                _logger.LogInformation("GRPC success {method} : {items} : {date}",
                                   nameof(OpenStockMovement),
                                   request.Items.Count,
                                   DateTime.UtcNow);
                return Task.FromResult(new OpenStockResponse() { Success = true });

            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(OpenStockMovement),
                                    DateTime.UtcNow);
                return Task.FromResult(new OpenStockResponse() { Success = false, Message = "Internal Error" });
            }
        }


        public override Task<CloseStockResponse> CloseStockMovement(CloseStockRequest request, ServerCallContext context)
        {
            try
            {
                if(!IsValidOrderType(request.Items.RelatedOrderType, out RelatedOrderType type))
                {
                    return Task.FromResult(new CloseStockResponse() { Success = false, Message = $"Order Type : {request.Items.RelatedOrderType}" });
                }

                List<StockMovement> mvmts = _repository.GetMovementsByOrder(type, request.Items.RelatedOrderId).ToList();

                if(!HasItems(mvmts))
                {
                    return Task.FromResult(new CloseStockResponse() { Success = false, Message = $"Order ID not valid : {request.Items.RelatedOrderId}" });
                }

                if(!_movementProcessor.Close(mvmts, out string error))
                {
                    return Task.FromResult(new CloseStockResponse() { Success = false, Message = error });
                }
   
                _repository.SaveChanges();

                _logger.LogInformation("GRPC success {method} : {type} : {id} : {date}",
                                   nameof(CloseStockMovement),
                                   type, 
                                   request.Items.RelatedOrderId,
                                   DateTime.UtcNow);
                return Task.FromResult(new CloseStockResponse() { Success = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(CloseStockMovement),
                                    DateTime.UtcNow);
                return Task.FromResult(new CloseStockResponse() { Success = false, Message = "Internal Error" });
            }
        }


        public override Task<CancelStockResponse> CancelStockMovement(CancelStockRequest request, ServerCallContext context)
        {
            try
            {
                if (!IsValidOrderType(request.Items.RelatedOrderType, out RelatedOrderType type))
                {
                    return Task.FromResult(new CancelStockResponse() { Success = false, Message = $"Order Type not valid : {request.Items.RelatedOrderType}" });
                }

                List<StockMovement> mvmts = _repository.GetMovementsByOrder(type, request.Items.RelatedOrderId).ToList();

                if (!HasItems(mvmts))
                {
                    return Task.FromResult(new CancelStockResponse() { Success = false, Message = $"Order ID not valid : {request.Items.RelatedOrderId}" });
                }

                if (!_movementProcessor.Cancel(mvmts, out string error))
                {
                    return Task.FromResult(new CancelStockResponse() { Success = false, Message = error });
                }

                _repository.SaveChanges();

                _logger.LogInformation("GRPC success {method} : {type} : {id} : {date}",
                                   nameof(CancelStockMovement),
                                   type,
                                   request.Items.RelatedOrderId,
                                   DateTime.UtcNow);
                return Task.FromResult(new CancelStockResponse() { Success = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(CancelStockMovement),
                                    DateTime.UtcNow);
                return Task.FromResult(new CancelStockResponse() { Success = false, Message = "Internal Error" });
            }
        }

        #region Private methods
        private bool IsValidOrderType(int orderType, out RelatedOrderType type)
        {
            return Enum.TryParse(orderType.ToString(), out type);
        }
        private bool HasItems(List<StockMovement> mvmts)
        {
            return mvmts.Count > 0;
        }
        #endregion
    }
}
