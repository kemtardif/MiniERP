using FluentValidation;
using MiniERP.PurchaseOrderService.Caching;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Validators
{
    public class PODetailValidator : AbstractValidator<PODetail>
    {
        private readonly ICacheRepository _cache;
        public PODetailValidator(ICacheRepository cache)
        {
            _cache = cache;

            RuleFor(x => x.Quantity)
                .Must(x => x > 0)
                .WithMessage("Quantity must be greater than zero")
                .WithName("Positive quantity");
        }
    }
}
