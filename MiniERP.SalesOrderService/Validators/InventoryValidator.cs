using FluentValidation;
using FluentValidation.Results;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Validators
{
    public class InventoryValidator : AbstractValidator<Inventory>
    {
        private readonly IDataClient _dataClient;

        public InventoryValidator(IDataClient dataClient)
        {
            _dataClient = dataClient;

            RuleFor(x => x)
                .Custom(ValidateInventory);
        }

        private void ValidateInventory(Inventory inventory, ValidationContext<Inventory> context)
        {
            foreach (InventoryItem item in inventory.Items)
            {
                StockResponse response = _dataClient.GetAvailableStock(item.Id);

                if (!response.IsFound)
                {
                    context.AddFailure(new ValidationFailure($"[{item.Id}]", "Item not found"));
                    continue;
                }

                if (response.Item.Status != 1)
                {
                    context.AddFailure(new ValidationFailure($"[{item.Id}]", "Item unavailable"));
                    continue;
                }

                if (response.Item.Quantity < item.Quantity)
                {
                    context.AddFailure(new ValidationFailure($"[{item.Id}]", "Quantity unavailable"));
                    continue;
                }
            }
        }
        }
    }
