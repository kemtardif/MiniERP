using FluentValidation;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class SalesOrderDetailValidator : AbstractValidator<SalesOrderDetail>
    {
        private ISalesOrderRepository _repository;

        public SalesOrderDetailValidator(ISalesOrderRepository repository)
        {
            _repository = repository;
        }
    }
}
