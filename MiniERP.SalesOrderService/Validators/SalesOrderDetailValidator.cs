using FluentValidation;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class SalesOrderDetailValidator : AbstractValidator<SalesOrderDetail>
    {
        public SalesOrderDetailValidator()
        {
            RuleFor(x =>  x.Quantity )
                    .Must(x => x > 0)
                    .WithMessage("Quantity must be greater than zero")
                    .WithName("Positive quantity");

        }
    }
}
