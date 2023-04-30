using FluentValidation;
using MiniERP.SalesOrderService.DTOs;

namespace MiniERP.SalesOrderService.Validators
{
    public class BaseValidator : AbstractValidator<SOCreateDTO>
    {
        public BaseValidator()
        {

            RuleFor(x => x.CustID).NotNull().Must(x => x > 0)
                .WithMessage("Customer Id must be greater than zero");
            RuleFor(x => x.CustAddress).NotEmpty();
            RuleFor(x => x.ConfirmDate).NotNull().Must(BeAValidDate)
                .WithMessage("Confirm Date must be a valid date");
            RuleFor(x => x.Details).NotNull().Must(x => x.Any())
                .WithMessage("Sales Order must have at least one line");
            RuleForEach(x => x.Details).SetValidator(new BaseDetailValidator());

        }

        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default);
        }


    }
    public class BaseDetailValidator : AbstractValidator<SODetailCreateDTO>
    {
        public BaseDetailValidator()
        {
            RuleFor(x => x.Quantity)
                    .Must(x => x > 0)
                    .WithMessage("Quantity must be greater than zero")
                    .WithName("Positive quantity");

        }
    }
}
