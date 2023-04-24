using AutoMapper;
using MiniERP.SalesOrderService.MessageBus.Responses;
using System.Text.Json;

namespace MiniERP.SalesOrderService.MessageBus
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
                case EventType.StockUpdated:
                    _logger.LogInformation("---> RabbitMQ : {name} received : {date}",
                                            EventType.StockUpdated, 
                                            DateTime.UtcNow);
                    await StockUpdated(message);
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
                MessageBusEventType.StockChanged => EventType.StockUpdated,
                _ => EventType.Undefined,
            };
        }     
        private async Task StockUpdated(string message)
        {
            try
            {
                StockChangedResponse? dto = DeserializeToStockCHangedResponse(message);
                if(dto is null)
                {
                    return;
                }
                await ProcessStockChangedResponse(dto);

            }
            catch(Exception ex)
            {
                HandleException(nameof(StockUpdated), ex);
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
        private StockChangedResponse? DeserializeToStockCHangedResponse(string message)
        {
            StockChangedResponse? resp = JsonSerializer.Deserialize<StockChangedResponse>(message);
            if (resp is null)
            {
                _logger.LogInformation("---> RabbitMQ : Deserialized dto is null : {date}",
                                    DateTime.UtcNow);
            }
            return resp;
        }
        private async Task ProcessStockChangedResponse(StockChangedResponse resp)
        {
            using var scope = _scopreFactory.CreateScope();

            //var cache = scope.ServiceProvider.GetRequiredService<ICacheRepository>();

            //foreach(StockChange change in resp.Changes)
            //{
            //    Stock? stock = await cache.GetStockByArticleId(change.Id);
            //    if (stock is null)
            //    {
            //        continue;
            //    }

            //    stock.Quantity = change.NewValue;

            //    await cache.SetStock(stock);

            //    _logger.LogInformation("---> RabbitMQ : Stock updated in cache: {id} : {date}",
            //                               stock.InventoryId,
            //                               DateTime.UtcNow);
            //}

        }
        #endregion
    }
    
}
