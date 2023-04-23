using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;
using System.Text.Json;

namespace MiniERP.SalesOrderService.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _repository;
        private readonly IValidator<SalesOrder> _validator;
        private readonly IMapper _mapper;
        private readonly IDataClient _dataClient;
        private readonly ILogger<SalesOrderService> _logger;

        public SalesOrderService(ISalesOrderRepository repository,
                                 IValidator<SalesOrder> validator,
                                 IMapper mapper,
                                 IDataClient dataClient,
                                 ILogger<SalesOrderService> logger)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _dataClient = dataClient;
            _logger = logger;
        }
        public async Task<Result<SalesOrderReadDto>> AddSalesOrder(SalesOrderCreateDto salesOrder)
        {
            var so = _mapper.Map<SalesOrder>(salesOrder);

            ValidationResult validationResult = await _validator.ValidateAsync(so);

            if (!validationResult.IsValid)
            {
                return Result<SalesOrderReadDto>.Failure(validationResult.ToDictionary());
            }

            var articleIds = so.Details.Select(x => x.ArticleId)
                                        .ToAsyncEnumerable();

            await foreach(StockResponse response in _dataClient.GetAvailableStockStream(articleIds))
            {
                ///Add Inventory logic here
            }

            _repository.AddSalesOrder(so);

            _repository.SaveChanges();

            var dto = _mapper.Map<SalesOrderReadDto>(so);

            return Result<SalesOrderReadDto>.Success(dto);
        }

        public Result<IEnumerable<SalesOrderReadDto>> GetAllSalesOrders()
        {
            IEnumerable<SalesOrder> sos = _repository.GetAllSalesOrders();

            var dtos = _mapper.Map<IEnumerable<SalesOrderReadDto>>(sos);

            return Result<IEnumerable<SalesOrderReadDto>>.Success(dtos);
        }

        public Result<SalesOrderReadDto> GetSalesOrderById(int id)
        {
            SalesOrder? so = _repository.GetSalesOrderById(id);

            if(so is null)
            {
                return Result<SalesOrderReadDto>.Failure(GetNotFoundResult(id));
            }

            var dto = _mapper.Map<SalesOrderReadDto>(so);

            return Result<SalesOrderReadDto>.Success(dto);
        }

        public Result RemoveSalesOrderById(int id)
        {
            SalesOrder? so = _repository.GetSalesOrderById(id);

            if(so is null)
            {
                return Result.Failure(GetNotFoundResult(id));
            }

            _repository.RemoveSalesOrder(so);

            _repository.SaveChanges();

            return Result.Success();
        }

        public async Task<Result<SalesOrderReadDto>> UpdateSalesOrder(int id, JsonPatchDocument<SalesOrderUpdateDto> json)
        {
            SalesOrder? so = _repository.GetSalesOrderById(id);

            if (so is null)
            {
                return Result<SalesOrderReadDto>.Failure(GetNotFoundResult(id));
            }

            try
            {
                _repository.UpdateSalesOrder(so, json);
            } 
            catch(Exception ex)
            {
                return Result<SalesOrderReadDto>.Failure(GetCaughtExceptionResult(ex.Message));
            }

            ValidationResult validationResult = await _validator.ValidateAsync(so);

            if (!validationResult.IsValid)
            {
                return Result<SalesOrderReadDto>.Failure(validationResult.ToDictionary());
            }

            _repository.SaveChanges();

            var dto = _mapper.Map<SalesOrderReadDto>(so);

            return Result<SalesOrderReadDto>.Success(dto);

        }

        #region Private Methods

        private IDictionary<string, string[]> GetNotFoundResult(int articleId)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(SalesOrder)] = new string[] { $"Sales Order not found : ID = {articleId}" }
            };
        }

        private IDictionary<string, string[]> GetCaughtExceptionResult(string message)
        {
            return new Dictionary<string, string[]>
            {
                ["message"] = new string[] { message }
            };
        }
        #endregion
    }
}
