using AutoMapper;
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
            GetStockRequest request = new()
            {
                ArticleId = articleId
            };

            GetStockResponse response = _client.GetStock(request);

            if(!response.IsFound)
            {
                return null;
            }
            return _mapper.Map<Stock>(response.Item);
        }

        public IEnumerable<Stock> StockChanged(IEnumerable<StockChangedModel> changed)
        {
            //StockChangedRequest request = new();
            //request.Items.AddRange(changed);

            //StockChangedResponse response = _client.StockChanged(request);

            //IEnumerable<Stock> stocks = _mapper.Map<IEnumerable<Stock>>(response.Items);

            //return stocks;
            throw new NotImplementedException();
        }
    }
}
