using FluentValidation;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class SODetailValidator : AbstractValidator<SalesOrderDetail>
    {
        public SODetailValidator()
        {
            RuleFor(x =>  x.Quantity)
                    .Must(x => x > 0)
                    .WithMessage("Quantity must be greater than zero")
                    .WithName("Positive quantity");

        }
    }
}
