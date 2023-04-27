using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Behaviors.CreateBehavior
{
    public class CreateValidationBehavior : IPipelineBehavior<CreateCommand, Result<ReadDTO>>
    {
        private readonly IValidator<CreateDTO> _validator;
        public CreateValidationBehavior(IValidator<CreateDTO> validator)
        {
            _validator = validator;
        }

        public async Task<Result<ReadDTO>> Handle(CreateCommand request, RequestHandlerDelegate<Result<ReadDTO>> next, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request.Item);

            if (!validationResult.IsValid)
            {
                return Result<ReadDTO>.Failure(validationResult.ToDictionary());
            }

            return await next();
        }
    }
}
