using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Exceptions;
using MiniERP.InventoryService.Models;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus
{
    public class RabbitMQProcessor : IMessageProcessor
    {
        private readonly IServiceScopeFactory _scopreFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<RabbitMQProcessor> _logger;

        public RabbitMQProcessor(IServiceScopeFactory scoporFactory, IMapper mapper, ILogger<RabbitMQProcessor> logger)
        {
            _scopreFactory = scoporFactory;
            _mapper = mapper;
            _logger = logger;
        }
        public void ProcessMessage(string message)
        {
            EventType type = GetEventType(message);

            switch(type)
            {
                case EventType.ArticleCreated:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}", 
                                            EventType.ArticleCreated, 
                                            DateTime.UtcNow);
                    ArticleCreated(message);
                    break;
                case EventType.ArticleDeleted:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
                                            EventType.ArticleDeleted,
                                            DateTime.UtcNow);
                    ArticleDeleted(message);
                    break;
                default:
                    return;
            }
        }

      
        private void ArticleCreated(string message)
        {
            using (var scope = _scopreFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                try
                {
                    StockEventDto? dto = JsonSerializer.Deserialize<StockEventDto>(message);
                    if(dto is null)
                    {
                        _logger.LogInformation("---> RabbitMQ : Deserialized dto is null : {date}",
                                            DateTime.UtcNow);
                        return;
                    }
                    Stock stock = _mapper.Map<Stock>(dto);
                    stock.UpdatedAt = stock.CreatedAt = DateTime.UtcNow;

                    repo.AddItem(stock);
                    repo.SaveChanges();

                    _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
                                           stock.ProductId,
                                           DateTime.UtcNow);
                }
                catch(JsonException ex)
                {
                    _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                   nameof(GetEventType),
                                   nameof(JsonException),
                                   ex.Message,
                                   DateTime.UtcNow);
                }
                catch(SaveChangesException sCEx) 
                {
                    _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                   nameof(GetEventType),
                                   nameof(SaveChangesException),
                                   sCEx.Message,
                                   DateTime.UtcNow);
                }

            }
        }

        private void ArticleDeleted(string message)
        {
            using (var scope = _scopreFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                try
                {
                    StockEventDto? dto = JsonSerializer.Deserialize<StockEventDto>(message);
                    if (dto is null)
                    {
                        _logger.LogInformation("---> RabbitMQ : Deserialized dto is null : {date}",
                                            DateTime.UtcNow);
                        return;
                    }
                    Stock? stock = repo.GetItemByArticleId(dto.Id);
                    if(stock is null)
                    {
                        _logger.LogInformation("---> RabbitMQ : Article for deletion not in stock : {id} : {date}",
                                               dto.Id,
                                               DateTime.UtcNow);
                        return;
                    }

                    repo.SetAsDiscontinued(stock);
                    stock.UpdatedAt = DateTime.UtcNow;

                    repo.SaveChanges();

                    _logger.LogInformation("---> RabbitMQ : Stock deleted for product: {id} : {date}",
                                           stock.ProductId,
                                           DateTime.UtcNow);
                }
                catch (JsonException ex)
                {
                    _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                   nameof(GetEventType),
                                   nameof(JsonException),
                                   ex.Message,
                                   DateTime.UtcNow);
                }
                catch (SaveChangesException sCEx)
                {
                    _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                   nameof(GetEventType),
                                   nameof(SaveChangesException),
                                   sCEx.Message,
                                   DateTime.UtcNow);
                }

            }
        }

        private EventType GetEventType(string message)
        {
            GenericEventDto? generic;
            try
            {
                generic = JsonSerializer.Deserialize<GenericEventDto>(message);

            }
            catch(JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                    nameof(GetEventType),
                                    nameof(JsonException),
                                    ex.Message,
                                    DateTime.UtcNow);
                return EventType.Undefined;
            }
            if(generic is null)
            {
                _logger.LogInformation("--->RabbitMQ : Deserialized message is null : {method} : {date}",
                                            nameof(GetEventType),
                                            DateTime.UtcNow);
                return EventType.Undefined;
            }

            return generic.EventName switch
            {
                "Article_created" => EventType.ArticleCreated,
                "Article_Deleted" => EventType.ArticleDeleted,
                _ => EventType.Undefined,
            };
        }
    }
    public enum EventType
    {
        ArticleCreated,
        ArticleDeleted,
        Undefined
    }
}
