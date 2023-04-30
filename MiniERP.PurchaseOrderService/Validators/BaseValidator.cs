using FluentValidation;
using MiniERP.PurchaseOrderService.DTOs;

namespace MiniERP.PurchaseOrderService.Validators
{
    public class BaseValidator : AbstractValidator<POCreateDTO>
    {
        public BaseValidator()
        {

                RuleFor(x => x.SupplierID).NotNull().Must(x => x > 0)
                    .WithMessage("SupplierId Id must be greater than zero");
                RuleFor(x => x.Status).NotNull().IsInEnum();
                RuleFor(x => x.DeliveryDate).NotNull().Must(BeAValidDate)
                    .WithMessage("Delivery Date must be a valid date");
                RuleFor(x => x.Details).NotNull().Must(x => x.Any())
                    .WithMessage("Sales Order must have at least one line");
                RuleForEach(x => x.Details).SetValidator(new DetailBaseValidator());
        }
        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default);
        }
    }

    public class DetailBaseValidator : AbstractValidator<PODetailCreateDTO>
    {
        public DetailBaseValidator()
        {

            RuleFor(x => x.Quantity)
                .Must(x => x > 0)
                .WithMessage("Quantity must be greater than zero")
                .WithName("Positive quantity");
        }
    }
}
