using AutoMapper;
using Grpc.Core;
using MiniERP.PurchaseOrderService.Extensions;
using MiniERP.PurchaseOrderService.Grpc.Protos;
using MiniERP.PurchaseOrderService.Models;
using Polly;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public class GRPCService : IRPCService
    {
        private const string RpcExceptionLogFormat = "----> RPC Exception for ID={id}";
        private const string ResultLogFormat = "---> RPC Result {result} for ID={id}";

        private readonly GrpcInventoryService.GrpcInventoryServiceClient _grpcClient;
        private readonly ILogger<GRPCService> _logger;
        private readonly IMapper _mapper;
        private readonly ISyncPolicy<StockResponse> _grpcPolicy;

        public GRPCService(GrpcInventoryService.GrpcInventoryServiceClient grpcClient,
                           ILogger<GRPCService> logger,
                           IMapper mapper,
                           ISyncPolicy<StockResponse> grpcPolicy)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _grpcPolicy = grpcPolicy ?? throw new ArgumentNullException(nameof(grpcPolicy));
        }


        public InventoryItem? GetInventoryById(int id)
        {
            InventoryItem? inventoryItem = null;
            try
            {
                StockRequest request = new() { ArticleId = id };
                var context = new Context().WithLogger<ILogger<GRPCService>>(_logger);

                StockResponse response = _grpcPolicy.Execute((cntx) =>
                    _grpcClient.GetInventory(request), context);

                if (response.IsFound)
                {
                    inventoryItem = _mapper.Map<InventoryItem>(response.Item);
                }
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, RpcExceptionLogFormat, id);
            }

            _logger.LogInformation(ResultLogFormat,
                                    inventoryItem is null ? "NOT FOUND" : "FOUND",
                                    id);
            return inventoryItem;
        }
    }
}
