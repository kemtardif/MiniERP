using FluentValidation;
using FluentValidation.Results;
using MiniERP.PurchaseOrderService.Caching;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Grpc;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Validators
{
    public class InventoryValidator : AbstractValidator<POCreateDTO>
    {

        private const string KeyFormat = "[{0}]";
        private const string NotFoundMessage = "Item not found";
        private const string UnavailableMessage = "Item unavailable";

        private readonly ICacheService _cacheService;
        private readonly IRPCService _rpcService;

        public InventoryValidator(ICacheService cacheService,
                                  IRPCService rpcService)
        {
            _cacheService = cacheService;
            _rpcService = rpcService;

            RuleFor(x => x)
                .Custom(ValidateInventory);
        }

        private void ValidateInventory(POCreateDTO create, ValidationContext<POCreateDTO> context)
        {
            foreach (ValidationItem item in GetValidationItems(create))
            {
                int id = item.Id;

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
            }
        }

        private List<ValidationItem> GetValidationItems(POCreateDTO create)
        {
            return create.Details
                        .GroupBy(x => x.Productd)
                        .Select(x => new ValidationItem() { Id = x.Key, Quantity = x.Sum(y => y.Quantity) })
                        .ToList();
        }
        private InventoryItem? GetInventoryItem(int id)
        {
            InventoryItem? item = _cacheService.GetInventoryById(id);
            item ??= _rpcService.GetInventoryById(id);

            return item;
        }
    }
    public class ValidationItem
    {
        public int Id { get; set; }
        public double Quantity { get; set; }
    }
}
