using FluentValidation;
using MiniERP.SalesOrderService.Caching;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class SalesOrderDetailValidator : AbstractValidator<SalesOrderDetail>
    {
        private ICacheRepository _cache;

        public SalesOrderDetailValidator(ICacheRepository repository)
        {
            _cache = repository;
            RuleFor(x =>  x.Quantity )
                    .Must(x => x > 0)
                    .WithMessage("Quantity must be greater than zero")
                    .WithName("Positive quantity");
            RuleFor(x => new { x.ArticleId, x.Quantity })
                    .MustAsync(async (x, y) => await HaveAvailableStock(x.ArticleId, x.Quantity))
                    .WithMessage("Quantity for product is above stock level")
                    .WithName("Available stock");
        }

        private async Task<bool> HaveAvailableStock(int articleId, double quantity)
        {
            Stock? stock = await _cache.GetStockByArticleId(articleId);

            if (stock is null) 
            {
                return false;
            }
            return quantity <= stock.Quantity;
        }

    }
}
