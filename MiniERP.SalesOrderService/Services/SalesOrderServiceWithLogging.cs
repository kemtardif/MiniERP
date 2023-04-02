using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Services
{
    public class SalesOrderServiceWithLogging : ISalesOrderService
    {
        private readonly ILogger<SalesOrderServiceWithLogging> _logger;
        private readonly ISalesOrderService _service;

        public SalesOrderServiceWithLogging(ILogger<SalesOrderServiceWithLogging> logger,
                                            ISalesOrderService salesOrderService)
        {
            _logger = logger;
            _service = salesOrderService;
        }
        public async Task<Result<SalesOrderReadDto>> AddSalesOrder(SalesOrderCreateDto salesOrder)
        {
            Result<SalesOrderReadDto> result = await _service.AddSalesOrder(salesOrder);

            if(result.IsSuccess)
            {
                _logger.LogInformation("Sales Order Created : Id = {id}, Date = {date}", result.Value.Id, DateTime.UtcNow);
            }

            return result;
        }

        public Result<IEnumerable<SalesOrderReadDto>> GetAllSalesOrders()
        {
            return _service.GetAllSalesOrders();
        }

        public Result<SalesOrderReadDto> GetSalesOrderById(int id)
        {
            return _service.GetSalesOrderById(id);
        }

        public Result RemoveSalesOrderById(int id)
        {
            Result result = _service.RemoveSalesOrderById(id);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Sales Order Deleted : Id = {id}, Date = {date}", id, DateTime.UtcNow);
            }

            return result;
        }

        public async Task<Result<SalesOrderReadDto>> UpdateSalesOrder(int id, JsonPatchDocument<SalesOrderUpdateDto> json)
        {
            Result<SalesOrderReadDto> result = await _service.UpdateSalesOrder(id, json);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Sales Order Updated : Id = {id}, Date = {date}", id, DateTime.UtcNow);
            }

            return result;
        }
    }
}
