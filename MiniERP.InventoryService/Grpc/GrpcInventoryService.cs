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

        public GrpcInventoryService(ILogger<GrpcInventoryService> logger,
                                        IMapper mapper,
                                        IInventoryRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        public override Task<StockResponse> GetInventory(StockRequest request, ServerCallContext context)
        {
            try
            {
                InventoryItem? inventory = _repository.GetInventoryByArticleId(request.ArticleId);

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

                    InventoryItem? inventory = _repository.GetInventoryByArticleId(callStream.Current.ArticleId);

                    if (inventory is null)
                    {
                        await responseStream.WriteAsync(new StockResponse() { ArticleId = callStream.Current.ArticleId, IsFound = false });
                        return;
                    }

                    StockModel stock = _mapper.Map<StockModel>(inventory);

                    await responseStream.WriteAsync(new StockResponse() { ArticleId = callStream.Current.ArticleId, IsFound = true, Item = stock });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GRPC error {method} : {date}",
                                    nameof(GetInventoryStream),
                                    DateTime.UtcNow);
                
            }
        }

        //public override Task<CloseStockResponse> CloseStockMovement(CloseStockRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        if (!IsValidOrderType(request.Items.RelatedOrderType, out RelatedOrderType type))
        //        {
        //            return Task.FromResult(new CloseStockResponse() { Success = false, Message = $"Order Type : {request.Items.RelatedOrderType}" });
        //        }

        //        List<InventoryMovement> mvmts = _repository.GetMovementsByOrder(type, request.Items.RelatedOrderId).ToList();

        //        if (!HasItems(mvmts))
        //        {
        //            return Task.FromResult(new CloseStockResponse() { Success = false, Message = $"Order ID not valid : {request.Items.RelatedOrderId}" });
        //        }

        //        if (!_movementProcessor.Close(mvmts, out string error))
        //        {
        //            return Task.FromResult(new CloseStockResponse() { Success = false, Message = error });
        //        }

        //        _repository.SaveChanges();

        //        _logger.LogInformation("GRPC success {method} : {type} : {id} : {date}",
        //                           nameof(CloseStockMovement),
        //                           type,
        //                           request.Items.RelatedOrderId,
        //                           DateTime.UtcNow);
        //        return Task.FromResult(new CloseStockResponse() { Success = true });

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "GRPC error {method} : {date}",
        //                            nameof(CloseStockMovement),
        //                            DateTime.UtcNow);
        //        return Task.FromResult(new CloseStockResponse() { Success = false, Message = "Internal Error" });
        //    }
        //}


        //public override Task<CancelStockResponse> CancelStockMovement(CancelStockRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        if (!IsValidOrderType(request.Items.RelatedOrderType, out RelatedOrderType type))
        //        {
        //            return Task.FromResult(new CancelStockResponse() { Success = false, Message = $"Order Type not valid : {request.Items.RelatedOrderType}" });
        //        }

        //        List<InventoryMovement> mvmts = _repository.GetMovementsByOrder(type, request.Items.RelatedOrderId).ToList();

        //        if (!HasItems(mvmts))
        //        {
        //            return Task.FromResult(new CancelStockResponse() { Success = false, Message = $"Order ID not valid : {request.Items.RelatedOrderId}" });
        //        }

        //        if (!_movementProcessor.Cancel(mvmts, out string error))
        //        {
        //            return Task.FromResult(new CancelStockResponse() { Success = false, Message = error });
        //        }

        //        _repository.SaveChanges();

        //        _logger.LogInformation("GRPC success {method} : {type} : {id} : {date}",
        //                           nameof(CancelStockMovement),
        //                           type,
        //                           request.Items.RelatedOrderId,
        //                           DateTime.UtcNow);
        //        return Task.FromResult(new CancelStockResponse() { Success = true });

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "GRPC error {method} : {date}",
        //                            nameof(CancelStockMovement),
        //                            DateTime.UtcNow);
        //        return Task.FromResult(new CancelStockResponse() { Success = false, Message = "Internal Error" });
        //    }
        //}

        //#region Private methods
        //private bool IsValidOrderType(int orderType, out RelatedOrderType type)
        //{
        //    return Enum.TryParse(orderType.ToString(), out type);
        //}
        //private bool HasItems(List<InventoryMovement> mvmts)
        //{
        //    return mvmts.Count > 0;
        //}
       // #endregion
    }
}
