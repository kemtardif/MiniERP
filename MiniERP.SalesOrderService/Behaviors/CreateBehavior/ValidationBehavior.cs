using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Behaviors.CreateBehavior
{
    public class ValidationBehavior : IPipelineBehavior<CreateCommand, Result<SOReadDTO>>
    {
        private readonly IValidator<SalesOrder> _baseValidator;
        private readonly IValidator<Inventory> _inventoryValidator;
        private readonly IMapper _mapper;
        public ValidationBehavior(IValidator<SalesOrder> baseValidator,
                                        IValidator<Inventory> inventoryValidator,
                                        IMapper mapper)
        {
            _baseValidator = baseValidator;
            _inventoryValidator = inventoryValidator;
            _mapper = mapper;
        }
        public async Task<Result<SOReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<SOReadDTO>> next, CancellationToken cancellationToken)
        {
            var salesOrder = _mapper.Map<SalesOrder>(request.SalesOrder);

            ValidationResult validationResult = _baseValidator.Validate(salesOrder);
            if (!validationResult.IsValid)
            {
                return Result<SOReadDTO>.Failure(validationResult.ToDictionary());
            }

            ValidationResult inventoryResult = _inventoryValidator.Validate(GetInventory(salesOrder));
            if (!inventoryResult.IsValid)
            {
                return Result<SOReadDTO>.Failure(inventoryResult.ToDictionary());
            }

            return await next();
        }

        private Inventory GetInventory(SalesOrder salesOrder)
        {

            var items = salesOrder.Details.GroupBy(x => x.ArticleId)
                                            .Select(y => new InventoryItem() { Id = y.Key, Quantity = y.Sum(z => z.Quantity) })
                                            .ToList();
            return new Inventory(items);
        }
    }
}
