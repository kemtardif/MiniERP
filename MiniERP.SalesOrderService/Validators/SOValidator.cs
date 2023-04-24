using FluentValidation;
using FluentValidation.Results;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class SOValidator : AbstractValidator<SalesOrder>
    {
        public SOValidator()
        {

            RuleFor(x => x.CustID).NotNull().Must(x => x > 0)
                .WithMessage("Customer Id must be greater than zero");
            RuleFor(x => x.CustAddress).NotEmpty();
            RuleFor(x => x.Status).NotNull().IsInEnum();
            RuleFor(x => x.ConfirmDate).NotNull().Must(BeAValidDate)
                .WithMessage("Confirm Date must be a valid date");
            RuleFor(x => x.Details).NotNull().Must(x => x.Any())
                .WithMessage("Sales Order must have at least one line");
            RuleForEach(x => x.Details).SetValidator(new SODetailValidator());

        }

        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default);
        }

    }
}
