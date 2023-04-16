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
 
            //switch (message.MessageName)
            //{
            //    case MessageBusEventType.ArticleCreated:
            //        _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
            //                                EventType.ArticleCreated,
            //                                DateTime.UtcNow);
            //        ArticleCreated(message);
            //        break;
            //    case MessageBusEventType.ArticleCreated:
            //        _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
            //                                EventType.ArticleDeleted,
            //                                DateTime.UtcNow);
            //        ArticleDeleted(message);
            //        break;
            //    case EventType.ArticleUpdated:
            //        _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
            //                                EventType.ArticleUpdated,
            //                                DateTime.UtcNow);
            //        ArticleUpdated(message);
            //        break;
            //    default:
            //        _logger.LogInformation("---->  NOTHING TO SEE PROCESS");
            //        return;
            //}
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


        //private void ArticleCreated(string message)
        //{
        //    try
        //    {
        //        ArticleResponse? dto = DeserializeToArticleResponse(message);
        //        if (dto is null)
        //        {
        //            return;
        //        }
        //        AddArticleResponse(dto);

        //    }
        //    catch (JsonException ex)
        //    {
        //        _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
        //                        nameof(GetEventType),
        //                        nameof(JsonException),
        //                        ex.Message,
        //                        DateTime.UtcNow);
        //    }
        //    catch (SaveChangesException sCEx)
        //    {
        //        _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
        //                        nameof(GetEventType),
        //                        nameof(SaveChangesException),
        //                        sCEx.Message,
        //                        DateTime.UtcNow);
        //    }
        //}

        //private void ArticleDeleted(string message)
        //{
        //    try
        //    {
        //        ArticleResponse? dto = DeserializeToArticleResponse(message);
        //        if (dto is null)
        //        {
        //            return;
        //        }

        //        RemoveArticleResponse(dto);
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

        //#region Helper Functions
        //private ArticleResponse? DeserializeToArticleResponse(string message)
        //{
        //    ArticleResponse? dto = JsonSerializer.Deserialize<ArticleResponse>(message);
        //    if (dto is null)
        //    {
        //        _logger.LogInformation("---> RabbitMQ : Deserialized dto is null : {date}",
        //                            DateTime.UtcNow);
        //    }
        //    return dto;
        //}
        //private void AddArticleResponse(ArticleResponse dto)
        //{
        //    using (var scope = _scopreFactory.CreateScope())
        //    {
        //        var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

        //        InventoryItem item = _mapper.Map<InventoryItem>(dto);
        //        item.Stock.Quantity = 150;
        //        item.MaxQuantity = 200;

        //        repo.AddInventoryItem(item);

        //        repo.SaveChanges();

        //        _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
        //                                       item.ProductId,
        //                                       DateTime.UtcNow);

        //    }
        //}
        //private void RemoveArticleResponse(ArticleResponse dto)
        //{
        //    using (var scope = _scopreFactory.CreateScope())
        //    {
        //        var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

        //        InventoryItem? item = repo.GetItemByArticleId(dto.Id, false, false);
        //        if (item is null)
        //        {
        //            return;
        //        }

        //        repo.SetAsDiscontinued(item);

        //        repo.SaveChanges();

        //        _logger.LogInformation("---> RabbitMQ : Stock deleted for product: {id} : {date}",
        //                                       item.ProductId,
        //                                       DateTime.UtcNow);
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
