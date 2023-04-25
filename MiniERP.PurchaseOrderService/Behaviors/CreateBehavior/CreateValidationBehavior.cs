using AutoMapper;
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
        private readonly IValidator<PurchaseOrder> _baseValidator;
        private readonly IMapper _mapper;
        public CreateValidationBehavior(IValidator<PurchaseOrder> baseValidator,                                  
                                        IMapper mapper)
        {
            _baseValidator = baseValidator;
            _mapper = mapper;
        }
        public async Task<Result<POReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<POReadDTO>> next, CancellationToken cancellationToken)
        {
            var salesOrder = _mapper.Map<PurchaseOrder>(request.PurchaseOrder);

            ValidationResult validationResult = _baseValidator.Validate(salesOrder);
            if (!validationResult.IsValid)
            {
                return Result<POReadDTO>.Failure(validationResult.ToDictionary());
            }

            return await next();
        }
    }
}
