using FluentValidation;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Validators
{
    public class PODetailValidator : AbstractValidator<PODetail>
    {
        public PODetailValidator()
        {

            RuleFor(x => x.Quantity)
                .Must(x => x > 0)
                .WithMessage("Quantity must be greater than zero")
                .WithName("Positive quantity");
        }
    }
}
