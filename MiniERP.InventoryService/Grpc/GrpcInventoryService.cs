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

        public GrpcInventoryService(IInventoryRepository repository,
                                    IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
    }
}
