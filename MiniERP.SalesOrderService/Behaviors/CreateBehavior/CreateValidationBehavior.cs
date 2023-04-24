using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Behaviors.CreateBehavior
{
    public class CreateValidationBehavior : IPipelineBehavior<CreateCommand, Result<SOReadDTO>>
    {
        private readonly IValidator<SalesOrder> _baseValidator;
        private readonly IMapper _mapper;
        private readonly IDataClient _dataCLient;
        private readonly ILogger<CreateValidationBehavior> _logger;

        public CreateValidationBehavior(IValidator<SalesOrder> baseValidator,
                                        IMapper mapper,
                                        IDataClient dataCLient,
                                        ILogger<CreateValidationBehavior> logger)
        {
            _baseValidator = baseValidator;
            _mapper = mapper;
            _dataCLient = dataCLient;
            _logger = logger;
        }
        public async Task<Result<SOReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<SOReadDTO>> next, CancellationToken cancellationToken)
        {
            var salesOrder = _mapper.Map<SalesOrder>(request.SalesOrder);

            ValidationResult validationResult = _baseValidator.Validate(salesOrder);
            if (!validationResult.IsValid)
            {
                return Result<SOReadDTO>.Failure(validationResult.ToDictionary());
            }

            ValidationResult inventoryResult = ValidateInventory(salesOrder);
            if (!inventoryResult.IsValid)
            {
                return Result<SOReadDTO>.Failure(inventoryResult.ToDictionary());
            }

            return await next();
        }

        private ValidationResult ValidateInventory(SalesOrder salesOrder)
        {

            var failures = new List<ValidationFailure>();

            foreach(SalesOrderDetail sod in salesOrder.Details)
            {
                StockResponse response = _dataCLient.GetAvailableStock(sod.ArticleId);

                if(!response.IsFound)
                {
                    failures.Add(new ValidationFailure($"ArticleID={sod.ArticleId}", $"Item not found"));
                    continue;
                }

                if (response.Item.Status != 1)
                {
                    failures.Add(new ValidationFailure($"ArticleID={sod.ArticleId}", $"Item is unavailable. Status= {response.Item.Status}"));
                    continue;
                }
                if(response.Item.Quantity < sod.Quantity)
                {
                    failures.Add(new ValidationFailure($"ArticleID={sod.ArticleId}", "Quantity is unavailable"));
                    continue;
                }
            }
                
            return new ValidationResult(failures);
        }
    }
}
