using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Exceptions;
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
                    return;
            }
        }

        #region Private Methods
        private EventType GetEventType(string message)
        {
            GenericResponse? generic;
            try
            {
                generic = JsonSerializer.Deserialize<GenericResponse>(message);

            }
            catch (Exception ex)
            {
                HandleException(nameof(GetEventType), ex);
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
        private void ArticleUpdated(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if(dto is null)
                {
                    return;
                }
                UpdateArticleResponse(dto);
                  
            }
            catch (Exception ex)
            {
                HandleException(nameof(ArticleUpdated), ex);
            }
        }

      
        private void ArticleCreated(string message)
        {
            try
            {
                ArticleResponse? dto = DeserializeToArticleResponse(message);
                if(dto is null)
                {
                    return;
                }
                AddArticleResponse(dto);

            }
            catch(Exception ex)
            {
                HandleException(nameof(ArticleCreated), ex);
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
                HandleException(nameof(ArticleDeleted), ex);
            }
        }
        
        private void HandleException(string method, Exception exception)
        {
            _logger.LogError("--->RabbitMQ Exception : {method} : {name} : {ex} : {date}",
                                    method,
                                    exception.GetType().Name,
                                    exception.Message,
                                    DateTime.UtcNow);
        }
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
            using var scope = _scopreFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            InventoryItem stock = _mapper.Map<InventoryItem>(dto);

            repo.AddInventoryItem(stock);
            repo.SaveChanges();

            _logger.LogInformation("---> RabbitMQ : New product saved has new Stock: {id} : {date}",
                                           stock.ProductId,
                                           DateTime.UtcNow);
        }
        private void RemoveArticleResponse(ArticleResponse dto)
        {
            using var scope = _scopreFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            InventoryItem? stock = GetScopedStockByArticleId(repo, dto.Id);
            if (stock is null)
            {
                return;
            }

            repo.SetAsClosed(stock);
            repo.SaveChanges();

            _logger.LogInformation("---> RabbitMQ : Stock deleted for product: {id} : {date}",
                                           stock.ProductId,
                                           DateTime.UtcNow);
        }
        private void UpdateArticleResponse(ArticleResponse dto)
        {
            using var scope = _scopreFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            InventoryItem? stock = GetScopedStockByArticleId(repo, dto.Id);
            if (stock is null)
            {
                return;
            }

            _mapper.Map(dto, stock);

            repo.SaveChanges();

            _logger.LogInformation("---> RabbitMQ : Stock updated from article : {id} : {date}",
                                           stock.ProductId,
                                           DateTime.UtcNow);
        }
        private InventoryItem? GetScopedStockByArticleId(IInventoryRepository repo, int id)
        {
            InventoryItem? stock = repo.GetItemByArticleId(id);
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
