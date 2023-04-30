using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Behaviors.UpdateBehavior
{
    public class UpdateValidationBehavior : IPipelineBehavior<UpdateCommand, Result<ReadDTO>>
    {
        private readonly IValidator<UpdateDTO> _validator;

        public UpdateValidationBehavior(IValidator<UpdateDTO> validator)
        {
            _validator = validator;
        }
        public async Task<Result<ReadDTO>> Handle(UpdateCommand request, RequestHandlerDelegate<Result<ReadDTO>> next, CancellationToken cancellationToken)
        {
            var valid = UpdateDTO.CreateValidDTO();

            try
            {
                request.Item.ApplyTo(valid);
            }
            catch (Exception ex)
            {
                return Result<ReadDTO>.Failure(GetCaughtExceptionResult(ex.Message));
            }

            request.Item.ApplyTo(valid);

            ValidationResult validationResult = _validator.Validate(valid);

            if(!validationResult.IsValid)
            {
                return Result<ReadDTO>.Failure(validationResult.ToDictionary());
            }

            return await next();
        }

        private IDictionary<string, string[]> GetCaughtExceptionResult(string message)
        {
            return new Dictionary<string, string[]>
            {
                ["message"] = new string[] { message }
            };
        }
    }
}
