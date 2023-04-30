using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Behaviors.CreateBehavior
{
    public class ValidationBehavior : IPipelineBehavior<CreateCommand, Result<SOReadDTO>>
    {
        private readonly IEnumerable<IValidator<SOCreateDTO>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<SOCreateDTO>> validators)
        {
            _validators = validators;
        }
        public async Task<Result<SOReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<SOReadDTO>> next, CancellationToken cancellationToken)
        {
            foreach(IValidator<SOCreateDTO> validator in _validators)
            {
                ValidationResult validationResult = validator.Validate(request.SalesOrder);
                if (!validationResult.IsValid)
                {
                    return Result<SOReadDTO>.Failure(validationResult.ToDictionary());
                }
            }

            return await next();
        }
    }
}
