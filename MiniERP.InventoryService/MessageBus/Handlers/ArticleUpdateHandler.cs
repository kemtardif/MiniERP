using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Handlers
{
    public class ArticleUpdateHandler : HandlerBase<ArticleUpdate>
    {
        private readonly IMapper _mapper;
        public ArticleUpdateHandler(ILogger<ArticleUpdate> logger,
                                    IMapper mapper,
                                    IInventoryRepository repository) : base(logger, repository)
        {
            _mapper = mapper;
        }

        protected override async Task ProtectedHandle(ArticleUpdate request)
        {
            InventoryItem? item = _repository.GetInventoryById(request.Id) ??
                throw new ArgumentException(string.Format(NotFoundLogFrmat, request.Id));

            _mapper.Map(request, item);

            _repository.Update(item);

            await _repository.SaveChanges();
        }
    }
}
