using AutoMapper;
using Grpc.Net.Client;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Protos;

namespace MiniERP.ArticleService.Grpc
{
    public class InventoryDataClient : IInventoryDataClient
    {
        private readonly IMapper _mapper;
        private readonly IGrpcClientAdapter _client;

        public InventoryDataClient(IMapper mapper,
                                   IGrpcClientAdapter client)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public void InventoryItemsCreated(IEnumerable<Article> articles)
        {

            InventoryItemRequest request = CreateInventoryItemRequest(articles);

            using var cts = new CancellationTokenSource();

            GrpcInventoryResponse response = _client.InventoryItemsCreated(request, cts.Token);

            if(response.ItemProcessed != articles.Count())
            {
                cts.Cancel();
            }

            cts.Token.ThrowIfCancellationRequested();
        }
        public void InventoryItemsUpdated(IEnumerable<Article> inventoryItems)
        {
            InventoryItemRequest request = CreateInventoryItemRequest(inventoryItems);

            using var cts = new CancellationTokenSource();


            GrpcInventoryResponse response = _client.InventoryItemsUpdated(request, cts.Token);

            if (response.ItemProcessed != inventoryItems.Count())
            {
                cts.Cancel();
            }

            cts.Token.ThrowIfCancellationRequested();
        }
        private InventoryItemRequest CreateInventoryItemRequest(IEnumerable<Article> articles)
        {
            var request = new InventoryItemRequest();
            request.InventoryItems.AddRange(_mapper.Map<IEnumerable<GrpcInventoryItemModel>>(articles));
            return request;
        }
    }
}
