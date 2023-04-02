using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Services
{
    public class SalesOrderServiceWithInventory : ISalesOrderService
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IInventoryDataClient _inventory;
        private readonly IMapper _mapper;

        public SalesOrderServiceWithInventory(ISalesOrderService salesOrderService,
                                              IInventoryDataClient inventory,
                                              IMapper mapper)
        {
            _salesOrderService = salesOrderService;
            _inventory = inventory;
            _mapper = mapper;
        }
        public async Task<Result<SalesOrderReadDto>> AddSalesOrder(SalesOrderCreateDto salesOrder)
        {
            Result<SalesOrderReadDto> result = await _salesOrderService.AddSalesOrder(salesOrder);

            if(result.IsSuccess)
            {
                var changed = _mapper.Map<IEnumerable<StockChangedModel>>(result.Value.Details);

                _ = _inventory.StockChanged(changed);
            }

            return result;
        }

        public Result<IEnumerable<SalesOrderReadDto>> GetAllSalesOrders()
        {
            return _salesOrderService.GetAllSalesOrders();
        }

        public Result<SalesOrderReadDto> GetSalesOrderById(int id)
        {
            return _salesOrderService.GetSalesOrderById(id);    
        }

        public Result RemoveSalesOrderById(int id)
        {
            return _salesOrderService.RemoveSalesOrderById(id);
        }

        public async Task<Result<SalesOrderReadDto>> UpdateSalesOrder(int id, JsonPatchDocument<SalesOrderUpdateDto> json)
        {
            Result<SalesOrderReadDto> result = await _salesOrderService.UpdateSalesOrder(id, json);

            if(result.IsSuccess)
            {
                var changed = _mapper.Map<IEnumerable<StockChangedModel>>(result.Value.Details);

                _ = _inventory.StockChanged(changed);
            }

            return result;
        }
    }
}
