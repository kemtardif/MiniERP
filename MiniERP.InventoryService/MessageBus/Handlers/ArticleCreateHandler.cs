using AutoMapper;
using MediatR;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class ArticleCreateHandler : HandlerBase<ArticleCreate>
    {
        private readonly IMapper _mapper;
        public ArticleCreateHandler(ILogger<ArticleCreate> logger,
                                    IMapper mapper,
                                    IRepository repository) : base(logger, repository)
        {
            _mapper = mapper;
        }

        protected override async Task ProtectedHandle(ArticleCreate article)
        {

            InventoryItem item = _mapper.Map<InventoryItem>(article);

            _repository.AddItem(item);

           await _repository.SaveChanges();
        }
    }
}
