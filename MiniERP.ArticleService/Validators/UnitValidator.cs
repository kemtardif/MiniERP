using FluentValidation;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Validators
{
    public class UnitValidator : AbstractValidator<Unit>
    {
        public UnitValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(5, 50);
            RuleFor(x => x.UnitCode).NotEmpty().Length(3, 5);
        }
    }
}
