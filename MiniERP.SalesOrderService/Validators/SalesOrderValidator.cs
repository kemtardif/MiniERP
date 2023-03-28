using FluentValidation;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class SalesOrderValidator : AbstractValidator<SalesOrder>
    {
        private ISalesOrderRepository _repository;

        public SalesOrderValidator(ISalesOrderRepository repository)
        {
            _repository = repository;
            RuleFor(x => x.CustID).NotNull().Must(x => x > 0);
            RuleFor(x => x.CustAddress).NotEmpty();
            RuleFor(x => x.Status).NotNull().IsInEnum();
            RuleFor(x => x.ConfirmDate).NotNull().Must(BeAValidDate)
                .WithMessage("Confirm Date must be a valid date");
            RuleFor(x => x.ConfirmDate).NotNull().Must(BeAValidDate)
                .WithMessage("Confirm Date must be a valid date");
            RuleFor(x => x.Details).NotNull();
            RuleForEach(x => x.Details).SetValidator(new SalesOrderDetailValidator(_repository));
        }
        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default);
        }
    }
}
