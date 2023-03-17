using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Exceptions;
using MiniERP.InventoryService.Extensions;
using MiniERP.InventoryService.MessageBus.Responses;
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
        public async Task ProcessMessage(string message)
        {
            EventType type = GetEventType(message);
            switch (type)
            {
                case EventType.ArticleCreated:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}", 
                                            EventType.ArticleCreated, 
                                            DateTime.UtcNow);
                    await ArticleCreated(message);
                    break;
                case EventType.ArticleDeleted:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
                                            EventType.ArticleDeleted,
                                            DateTime.UtcNow);
                    await ArticleDeleted(message);
                    break;
                case EventType.ArticleUpdated:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
                                            EventType.ArticleUpdated,
                                            DateTime.UtcNow);
                    await ArticleUpdated(message);
                    break;
                default:
                    _logger.LogInformation("---->  NOTHING TO SEE PROCESS");
                    return;
            }
        }
        private async Task ArticleUpdated(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if(dto is null)
                {
                    return;
                }
                await UpdateArticleResponse(dto);
                  
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

      
        private async Task ArticleCreated(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if(dto is null)
                {
                    return;
                }
                await AddArticleResponse(dto);

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
       
        private async Task ArticleDeleted(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if (dto is null)
                {
                    return;
                }

                await RemoveArticleResponse(dto);
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
            GenericEvent? generic;
            try
            {
                generic = JsonSerializer.Deserialize<GenericEvent>(message);

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
        private async Task AddArticleResponse(ArticleResponse dto)
        {
            await using (var scope = _scopreFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                Stock stock = _mapper.Map<Stock>(dto);

                repo.AddItem(stock);

                _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
                                               stock.ProductId,
                                               DateTime.UtcNow);

            }
        }
        private async Task RemoveArticleResponse(ArticleResponse dto)
        {
            await using(var scope = _scopreFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                Stock? stock = await GetScopedStockByArticleId(repo, dto.Id);
                if (stock is null)
                {
                    return;
                }

                await repo.SetAsDiscontinued(stock);

                _logger.LogInformation("---> RabbitMQ : Stock deleted for product: {id} : {date}",
                                               stock.ProductId,
                                               DateTime.UtcNow);
            }
        }
        private async Task UpdateArticleResponse(ArticleResponse dto)
        {
            await using(var scope = _scopreFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                Stock? stock = await GetScopedStockByArticleId(repo, dto.Id);
                if (stock is null)
                {
                    return;
                }

                await repo.UpdateFromMessage(stock, dto);


                _logger.LogInformation("---> RabbitMQ : Stock updated from article : {id} : {date}",
                                               stock.ProductId,
                                               DateTime.UtcNow);
            }
        }
        private async Task<Stock?> GetScopedStockByArticleId(IInventoryRepository repo, int id)
        {
            Stock? stock = await repo.GetItemByArticleId(id);
            if (stock is null)
            {
                _logger.LogWarning("---> RabbitMQ : Article for update not in stock : {id} : {date}",
                                       id,
                                       DateTime.UtcNow);
            }
            return stock;
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
