using FluentValidation;
using MiniERP.PurchaseOrderService.Caching;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Validators
{
    public class POValidator : AbstractValidator<PurchaseOrder>
    {
        public POValidator(ICacheRepository cache)
        {

                RuleFor(x => x.SupplierID).NotNull().Must(x => x > 0)
                    .WithMessage("SupplierId Id must be greater than zero");
                RuleFor(x => x.Status).NotNull().IsInEnum();
                RuleFor(x => x.DeliveryDate).NotNull().Must(BeAValidDate)
                    .WithMessage("Delivery Date must be a valid date");
                RuleFor(x => x.Details).NotNull().Must(x => x.Any())
                    .WithMessage("Sales Order must have at least one line");
                RuleForEach(x => x.Details).SetValidator(new PODetailValidator(cache));
        }
        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default);
        }
    }
}
