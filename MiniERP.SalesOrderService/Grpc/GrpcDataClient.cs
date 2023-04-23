using AutoMapper;
using Grpc.Core;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
{
    public class GrpcDataClient : IDataClient
    {
        private readonly GrpcInventoryService.GrpcInventoryServiceClient _grpcClient;

        public GrpcDataClient(GrpcInventoryService.GrpcInventoryServiceClient grpcClient, 
                              ILogger<GrpcDataClient> logger,
                              IMapper mapper)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public StockResponse GetAvailableStock(int articleId)
        {
            StockRequest request = new() {  ArticleId = articleId };

            StockResponse response = _grpcClient.GetInventory(request);

            return response;
        }

        public async IAsyncEnumerable<StockResponse> GetAvailableStockStream(IAsyncEnumerable<int> articleIds)
        {
            using var tcs = new CancellationTokenSource();
            using var stream = _grpcClient.GetInventoryStream(cancellationToken: tcs.Token);

            var requestTask = Task.Run(async () =>
            {
                await foreach(int articleId in articleIds)
                {
                    StockRequest request = new() { ArticleId = articleId };

                    await stream.RequestStream.WriteAsync(request);
                };

                await stream.RequestStream.CompleteAsync();
            });

            await foreach(StockResponse response in stream.ResponseStream.ReadAllAsync())
            {
                yield return response;
            }

            await requestTask;
        }
    }
}
