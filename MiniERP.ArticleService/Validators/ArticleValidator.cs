using FluentValidation;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Validators
{
    public class ArticleValidator : AbstractValidator<Article>
    {
        private IArticleRepository _repo;
        public ArticleValidator(IArticleRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.EAN).NotNull().Must(x => x > 0);
            RuleFor(x => x.Name).NotEmpty().Length(5, 50);
            RuleFor(x => x.Description).NotNull().Length(0, 255);
            RuleFor(x => x.Type).NotNull().IsInEnum();
            RuleFor(x => x.Status).NotNull().IsInEnum();
            RuleFor(x => x.BasePrice).NotNull().Must(x => x >= 0)
                        .WithMessage("BasePrice must be non-negative");
            RuleFor(x => x.BaseUnitId).NotNull().Custom((id, context) =>
            {
                if (!_repo.HasValidUnits(id))
                {
                    context.AddFailure("Must have valid BaseUnitId");
                }
            });
            RuleFor(x => x.MaxQuantity).NotNull().Must(x => x >= 0);
            RuleFor(x => x.AutoOrder).NotNull();
            RuleFor(x => x.AutoTreshold).NotNull().Must(x => x >= 0)
                .WithMessage("AutoTreshold must be non-negative");
            RuleFor(x => x.AutoQuantity).NotNull().Must(x => x >= 0)
                .WithMessage("AutoQuantity must be non-negative"); ;
            RuleFor(x => new { x.AutoTreshold, x.AutoQuantity, x.MaxQuantity })
                    .Must(x => x.AutoTreshold + x.AutoQuantity <= x.MaxQuantity)
                    .WithMessage("Sum of AutoTreshold and AutoQuantity must be smaller or equal to MaxQuantity")
                    .WithName("Auto-order consistency");
        }
    }
}
