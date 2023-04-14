using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Exceptions;
using System.Text.Json;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.MessageBus.Subscriber
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
            switch (type)
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
                case EventType.ArticleUpdated:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
                                            EventType.ArticleUpdated,
                                            DateTime.UtcNow);
                    ArticleUpdated(message);
                    break;
                default:
                    _logger.LogInformation("---->  NOTHING TO SEE PROCESS");
                    return;
            }
        }
        private void ArticleUpdated(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if (dto is null)
                {
                    return;
                }
                UpdateArticleResponse(dto);

            }
            catch (JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                nameof(GetEventType),
                                nameof(JsonException),
                                ex.Message,
                                DateTime.UtcNow);
            }
            catch (DbUpdateException sCEx)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                nameof(GetEventType),
                                nameof(DbUpdateException),
                                sCEx.Message,
                                DateTime.UtcNow);
            }
        }


        private void ArticleCreated(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if (dto is null)
                {
                    return;
                }
                AddArticleResponse(dto);

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

        private void ArticleDeleted(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if (dto is null)
                {
                    return;
                }

                RemoveArticleResponse(dto);
            }
            catch (JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                nameof(GetEventType),
                                nameof(JsonException),
                                ex.Message,
                                DateTime.UtcNow);
            }
            catch (DbUpdateException sCEx)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                nameof(GetEventType),
                                nameof(DbUpdateException),
                                sCEx.Message,
                                DateTime.UtcNow);
            }
        }

        private EventType GetEventType(string message)
        {
            GenericResponse? generic;
            try
            {
                generic = JsonSerializer.Deserialize<GenericResponse>(message);

            }
            catch (JsonException ex)
            {
                _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                    nameof(GetEventType),
                                    nameof(JsonException),
                                    ex.Message,
                                    DateTime.UtcNow);
                return EventType.Undefined;
            }
            if (generic is null)
            {
                _logger.LogInformation("--->RabbitMQ : Deserialized message is null : {method} : {date}",
                                            nameof(GetEventType),
                                            DateTime.UtcNow);
                return EventType.Undefined;
            }

            return generic.EventName switch
            {
                MessageBusEventType.ArticleCreated => EventType.ArticleCreated,
                MessageBusEventType.ArticleDeleted => EventType.ArticleDeleted,
                MessageBusEventType.ArticleUpdated => EventType.ArticleUpdated,
                _ => EventType.Undefined,
            };
        }

        #region Helper Functions
        private ArticleResponse? DeserializeToArticleResponse(string message)
        {
            ArticleResponse? dto = JsonSerializer.Deserialize<ArticleResponse>(message);
            if (dto is null)
            {
                _logger.LogInformation("---> RabbitMQ : Deserialized dto is null : {date}",
                                    DateTime.UtcNow);
            }
            return dto;
        }
        private void AddArticleResponse(ArticleResponse dto)
        {
            using (var scope = _scopreFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem item = _mapper.Map<InventoryItem>(dto);
                item.Stock.Quantity = 150;
                item.MaxQuantity = 200;

                repo.AddInventoryItem(item);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
                                               item.ProductId,
                                               DateTime.UtcNow);

            }
        }
        private void RemoveArticleResponse(ArticleResponse dto)
        {
            using (var scope = _scopreFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem? item = repo.GetItemByArticleId(dto.Id);
                if (item is null)
                {
                    return;
                }

                repo.SetAsDiscontinued(item);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : Stock deleted for product: {id} : {date}",
                                               item.ProductId,
                                               DateTime.UtcNow);
            }
        }
        private void UpdateArticleResponse(ArticleResponse dto)
        {
            using (var scope = _scopreFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                InventoryItem? item = repo.GetItemByArticleId(dto.Id);
                if (item is null)
                {
                    return;
                }

                _mapper.Map(dto, item);

                repo.SaveChanges();

                _logger.LogInformation("---> RabbitMQ : Stock updated from article : {id} : {date}",
                                               item.ProductId,
                                               DateTime.UtcNow);
            }
        }
        #endregion
    }
    public enum EventType
    {
        ArticleCreated,
        ArticleDeleted,
        ArticleUpdated,
        Undefined
    }
}
