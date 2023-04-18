using AutoMapper;
using System.Text.Json;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.MessageBus.Processors;

namespace MiniERP.InventoryService.MessageBus.Subscriber
{
    public class RabbitMQRouter : IMessageRouter
    {
        private readonly IServiceScopeFactory _scopreFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<RabbitMQRouter> _logger;
        private readonly Dictionary<string, IMessageProcessor> _processors = new();

        public RabbitMQRouter(IServiceScopeFactory scopeFactory, 
                              IMapper mapper, 
                              ILogger<RabbitMQRouter> logger)
        {
            _scopreFactory = scopeFactory;
            _mapper = mapper;
            _logger = logger;


            var scope = scopeFactory.CreateScope();
            IEnumerable<IMessageProcessor> processors = scope.ServiceProvider.GetServices<IMessageProcessor>();

            foreach(IMessageProcessor processor in processors)
            {
                _processors.Add(processor.ServiceType, processor);
            }
        }
        public void RouteMessage(IMessage message, string data)
        {
            if(_processors.ContainsKey(message.EventName))
            {
                _processors[message.EventName].ProcessMessage(data);
            }
            else
            {
                _logger.LogInformation("---> RabbitMQ : No processor found for message {message} : {date}",
                                            message.EventName,
                                            DateTime.UtcNow);
            }
        }

        private IMessage GetEvent(string message)
        {

            try
            {
                IMessage? eventMessage = JsonSerializer.Deserialize<IMessage>(message);

                if(eventMessage is null)
                {
                    return new UndefinedMessage();
                }
                return eventMessage;

            }
            catch (JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                    nameof(GetEvent),
                                    nameof(JsonException),
                                    ex.Message,
                                    DateTime.UtcNow);

                return new UndefinedMessage();
            }
        }

        //private void ArticleUpdated(string message)
        //{
        //    try
        //    {
        //        ArticleResponse? dto = DeserializeToArticleResponse(message);
        //        if (dto is null)
        //        {
        //            return;
        //        }
        //        UpdateArticleResponse(dto);

        //    }
        //    catch (JsonException ex)
        //    {
        //        _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
        //                        nameof(GetEventType),
        //                        nameof(JsonException),
        //                        ex.Message,
        //                        DateTime.UtcNow);
        //    }
        //    catch (DbUpdateException sCEx)
        //    {
        //        _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
        //                        nameof(GetEventType),
        //                        nameof(DbUpdateException),
        //                        sCEx.Message,
        //                        DateTime.UtcNow);
        //    }
        //}

       
        //private void UpdateArticleResponse(ArticleResponse dto)
        //{
        //    using (var scope = _scopreFactory.CreateScope())
        //    {
        //        var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

        //        InventoryItem? item = repo.GetItemByArticleId(dto.Id, false, false);
        //        if (item is null)
        //        {
        //            return;
        //        }

        //        _mapper.Map(dto, item);

        //        repo.SaveChanges();

        //        _logger.LogInformation("---> RabbitMQ : Stock updated from article : {id} : {date}",
        //                                       item.ProductId,
        //                                       DateTime.UtcNow);
        //    }
        //}
       // #endregion
    }
    public enum EventType
    {
        ArticleCreated,
        ArticleDeleted,
        ArticleUpdated,
        Undefined
    }
}
