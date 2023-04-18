using AutoMapper;
using MiniERP.InventoryService.MessageBus.Messages;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Processors
{
    public abstract class ArticleProcessorBase : IMessageProcessor
    {
        public abstract string ServiceType { get; }

        protected readonly ILogger<ArticleProcessorBase> _logger;
        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly IMapper _mapper;
        public ArticleProcessorBase(ILogger<ArticleProcessorBase> logger,
                                    IServiceScopeFactory scopeFactory,
                                    IMapper mapper)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public abstract void ProcessMessage(string data);

        protected ArticleMessage? DeserializeToArticle(string data)
        {
            return JsonSerializer.Deserialize<ArticleMessage>(data);
        }
    }
}
