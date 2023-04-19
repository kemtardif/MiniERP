using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Services
{
    public class SalesOrderServiceWithGrpc : ISalesOrderService
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IGrpcClientAdapter _grpcCLient;
        private readonly IMapper _mapper;

        public SalesOrderServiceWithGrpc(ISalesOrderService salesOrderService,
                                        IGrpcClientAdapter grpcCLient,
                                        IMapper mapper)
        {
            _salesOrderService = salesOrderService;
            _grpcCLient = grpcCLient;
            _mapper = mapper;
        }
        public async Task<Result<SalesOrderReadDto>> AddSalesOrder(SalesOrderCreateDto salesOrder)
        {
            Result<SalesOrderReadDto> result = await _salesOrderService.AddSalesOrder(salesOrder);

            if (result.IsSuccess)
            {
                OpenStockRequest request = new();

                int orderId = result.Value.Id;
                foreach (SalesOrderDetailReadDto line in result.Value.Details)
                {
                    OpenStockModel model = _mapper.Map<OpenStockModel>(line);

                    model.RelatedOrderId = orderId;

                    request.Items.Add(model);
                }

                _ = _grpcCLient.OpenStockMovement(request);
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

            //if (result.IsSuccess)
            //{
            //    var changed = _mapper.Map<IEnumerable<StockChangedModel>>(result.Value.Details);

            //    _ = _grpcCLient.StockChanged(changed);
            //}

            return result;
        }

        #region Private Methods
        private IDictionary<string, string[]> GetGrpcError(string error)
        {
            return new Dictionary<string, string[]>() { { "grpc", new string[] { error } } };
        }
        #endregion
    }
}
