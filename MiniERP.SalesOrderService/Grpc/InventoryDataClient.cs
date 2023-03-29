using AutoMapper;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Grpc
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

        public Stock? GetStockByArticleId(int articleId)
        {
            StockRequest request = new StockRequest()
            {
                Id = articleId
            };

            StockResponse response = _client.GetStockByArticleId(request);

            if(!response.IsFound)
            {
                return null;
            }
            return _mapper.Map<Stock>(response.Item);
        }
    }
}
