using FluentValidation;
using MiniERP.ArticleService.DTOs;

namespace MiniERP.ArticleService.Validators
{
    public class CreateValidator : AbstractValidator<CreateDTO>
    {
        public CreateValidator()
        {

            RuleFor(x => x.EAN).NotNull().Must(x => x >= 0)
                .WithMessage("EAN must be a non-negative number");
            RuleFor(x => x.Name).NotEmpty().Length(5, 50)
                .WithMessage("Name must be between 5 and 50 characters");
            RuleFor(x => x.Description).NotNull().Length(0, 255)
                .WithMessage("Description must be between 0 and 255 characters");
            RuleFor(x => x.Type).NotNull().IsInEnum()
                .WithMessage("Article type is not valid");
            RuleFor(x => x.Status).NotNull().IsInEnum()
                .WithMessage("Status is not valid");
            RuleFor(x => x.BasePrice).NotNull().Must(x => x >= 0)
                        .WithMessage("BasePrice must be non-negative");
            RuleFor(x => x.AutoOrder).NotNull();
            RuleFor(x => x.AutoTreshold).NotNull().Must(x => x >= 0)
                .WithMessage("AutoTreshold must be non-negative");
            RuleFor(x => x.AutoQuantity).NotNull().Must(x => x >= 0)
                .WithMessage("AutoQuantity must be non-negative");
        }
    }
}
