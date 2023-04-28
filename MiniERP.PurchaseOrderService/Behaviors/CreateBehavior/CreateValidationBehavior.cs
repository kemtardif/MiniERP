using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Behaviors.CreateBehavior
{
    public class CreateValidationBehavior : IPipelineBehavior<CreateCommand, Result<POReadDTO>>
    {
        private readonly IEnumerable<IValidator<POCreateDTO>> _validators;
        public CreateValidationBehavior(IEnumerable<IValidator<POCreateDTO>> validators)
        {
            _validators = validators;
        }
        public async Task<Result<POReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<POReadDTO>> next, CancellationToken cancellationToken)
        {
            foreach(IValidator<POCreateDTO> validator in _validators)
            {
                ValidationResult validationResult = validator.Validate(request.PurchaseOrder);
                if (!validationResult.IsValid)
                {
                    return Result<POReadDTO>.Failure(validationResult.ToDictionary());
                }
            }

            return await next();
        }
    }
}
