using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class ArticleDeleteHandler : HandlerBase<ArticleDelete>
    {
        public ArticleDeleteHandler(ILogger<ArticleDelete> logger, IInventoryRepository repository) 
            : base(logger, repository) { }

        protected override async Task ProtectedHandle(ArticleDelete request)
        {
            _repository.CloseItem(request.Id);

            await _repository.SaveChanges();
        }
    }
}
