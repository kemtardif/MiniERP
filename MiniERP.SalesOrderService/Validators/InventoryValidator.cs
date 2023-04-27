using FluentValidation;
using FluentValidation.Results;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Services.Contracts;

namespace MiniERP.SalesOrderService.Validators
{
    public class InventoryValidator : AbstractValidator<SOCreateDTO>
    {
        private readonly ICacheService _cacheService;
        private readonly IRPCService _rpcService;

        public InventoryValidator(ICacheService cacheService,
                                  IRPCService rpcService)
        {
            _cacheService= cacheService;
            _rpcService= rpcService;

            RuleFor(x => x)
                .Custom(ValidateInventory);
        }

        private void ValidateInventory(SOCreateDTO create, ValidationContext<SOCreateDTO> context)
        {
            foreach (SODetailCreateDTO item in create.Details)
            {
                InventoryItem? model = GetInventoryItem(item.ArticleId);

                if (model is null)
                {
                    context.AddFailure(new ValidationFailure($"[{item.ArticleId}]", "Item not found"));
                    continue;
                }

                if (model.Status != 1)
                {
                    context.AddFailure(new ValidationFailure($"[{item.ArticleId}]", "Item unavailable"));
                    continue;
                }

                if (model.Quantity < item.Quantity)
                {
                    context.AddFailure(new ValidationFailure($"[{item.ArticleId}]", "Quantity unavailable"));
                    continue;
                }
            }
        }
        private InventoryItem? GetInventoryItem(int id)
        {
            InventoryItem? item = _cacheService.GetItemById(id);
            item ??= _rpcService.GetItemById(id);

            return item;
        }
    }
}
