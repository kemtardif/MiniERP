using AutoMapper;
using Grpc.Core;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Sender;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Grpc
{
    public class GrpcInventoryService : GrpcInventory.GrpcInventoryBase
    {
        private readonly IInventoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusSender _sender;

        public GrpcInventoryService(IInventoryRepository repository,
                                    IMapper mapper,
                                    IMessageBusSender sender)
        {
            _repository = repository;
            _mapper = mapper;
            _sender = sender;
        }
        public override Task<StockResponse> GetStockByArticleId(StockRequest request, ServerCallContext context)
        {
            var response = new StockResponse();

            InventoryItem? item = _repository.GetItemByArticleId(request.Id);

            if(item is not null)
            {
                response.IsFound = true;
                response.Item = _mapper.Map<StockModel>(item);
            }

            return Task.FromResult(response);
        }

        public override Task<StockChangedResponse> StockChanged(StockChangedRequest request, ServerCallContext context)
        {
            var response = new StockChangedResponse();

            foreach(var item in request.Items)
            {
                InventoryItem? invItem = _repository.GetItemByArticleId(item.Id);

                if(invItem is null)
                {
                    continue;
                }

                switch (item.ChangeType)
                {
                    case 1:
                        invItem.Stock.Quantity += item.Value;
                        break;
                    case 2:
                        invItem.Stock.Quantity -= item.Value;
                        break;
                    default:
                        continue;
                }

                response.Items.Add(_mapper.Map<StockModel>(invItem));
            }
            _repository.SaveChanges();

            if(response.Items.Count > 0)
            {
                _sender.RequestForPublish<StockModel>(RequestType.StockUpdated, response.Items);
            }

            return Task.FromResult(response);
        }

    }
}
