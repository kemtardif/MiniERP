using FluentValidation;
using FluentValidation.Results;
using MiniERP.SalesOrderService.Caching;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Validators
{
    public class InventoryValidator : AbstractValidator<SOCreateDTO>
    {

        private const string KeyFormat = "[{0}]";
        private const string NotFoundMessage = "Item not found";
        private const string UnavailableMessage = "Item unavailable";
        private const string QuantityMessage = "Quantity unavailable";

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
                int id = item.ArticleId;
                InventoryItem? model = GetInventoryItem(id);

                if (model is null)
                {
                    context.AddFailure(new ValidationFailure(string.Format(KeyFormat, id), NotFoundMessage));
                    continue;
                }
                if (model.Status != 1)
                {
                    context.AddFailure(new ValidationFailure(string.Format(KeyFormat, id), UnavailableMessage));
                    continue;
                }
                if (model.Quantity < item.Quantity)
                {
                    context.AddFailure(new ValidationFailure(string.Format(KeyFormat, id), QuantityMessage));
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
