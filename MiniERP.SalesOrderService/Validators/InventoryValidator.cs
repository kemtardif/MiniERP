using FluentValidation;
using FluentValidation.Results;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;
using StackExchange.Redis;

namespace MiniERP.SalesOrderService.Validators
{
    public class InventoryValidator : AbstractValidator<Inventory>
    {
        private readonly ICache _cache;

        public InventoryValidator(ICache cache)
        {
            _cache = cache;

            RuleFor(x => x)
                .Custom(ValidateInventory);
        }

        private void ValidateInventory(Inventory inventory, ValidationContext<Inventory> context)
        {
            foreach (InventoryItem item in inventory.Items)
            {
                StockModel? model = _cache.GetCachedStockModel(item.Id);

                if (model is null)
                {
                    context.AddFailure(new ValidationFailure($"[{item.Id}]", "Item not found"));
                    continue;
                }

                if (model.Status != 1)
                {
                    context.AddFailure(new ValidationFailure($"[{item.Id}]", "Item unavailable"));
                    continue;
                }

                if (model.Quantity < item.Quantity)
                {
                    context.AddFailure(new ValidationFailure($"[{item.Id}]", "Quantity unavailable"));
                    continue;
                }
            }
        }
        }
    }
