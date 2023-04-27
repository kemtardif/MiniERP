using AutoMapper;
using Grpc.Core;
using MiniERP.SalesOrderService.Extensions;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;
using MiniERP.SalesOrderService.Services.Contracts;
using Polly;

namespace MiniERP.SalesOrderService.Services
{
    public class GRPCService : IRPCService
    {
        private const string RpcExceptionLogFormat = "----> RPC Exception for ID={id}";
        private const string ResultLogFormat = "---> RPC Result {result} for ID={id}";

        private readonly GrpcInventoryService.GrpcInventoryServiceClient _grpcClient;
        private readonly ISyncPolicy<StockResponse> _grpcPolicy;
        private readonly IMapper _mapper;
        private readonly ILogger<GRPCService> _logger;

        public GRPCService(GrpcInventoryService.GrpcInventoryServiceClient grpcClient,
                          ISyncPolicy<StockResponse> grpcPolicy,
                          IMapper mapper,
                          ILogger<GRPCService> logger)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
            _grpcPolicy = grpcPolicy ?? throw new ArgumentNullException(nameof(grpcPolicy)); 
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public InventoryItem? GetItemById(int id)
        {
            InventoryItem? item = null;
            try
            {
                var request = new StockRequest() { ArticleId = id };
                var context = new Context().WithLogger<ILogger<GRPCService>>(_logger);

                StockResponse response = _grpcPolicy.Execute((cntx) =>
                    _grpcClient.GetInventory(request), context);

                if (response.IsFound)
                {
                    item =  _mapper.Map<InventoryItem>(response.Item);
                }
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, RpcExceptionLogFormat, id);
            }

            _logger.LogInformation(ResultLogFormat,
                                    item is null ? "NOT FOUND" : "FOUND",
                                    id);
            return item;
        }
    }
}
