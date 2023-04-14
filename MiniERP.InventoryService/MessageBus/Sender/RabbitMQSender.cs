using AutoMapper;
using MiniERP.InventoryService.MessageBus.Events;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public class RabbitMQSender : IMessageBusSender
    {
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _client;

        public RabbitMQSender(IMapper mapper,
                              IMessageBusClient client)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public void RequestForPublish<T>(RequestType type, IEnumerable<T> item)
        {
            string eventName = GetEventName(type);

            IEnumerable<GenericEvent> events = GetEvents(type, eventName, item);

            foreach (GenericEvent evnt in events)
            {
                _client.Publish(evnt);
            }
        }

        private string GetEventName(RequestType type)
        {
            return type switch
            {
                RequestType.StockUpdated => MessageBusEventType.StockChanged,
                _ => throw new ArgumentException(nameof(RequestType)),
            };
        }
        private IEnumerable<GenericEvent> GetEvents<T>(RequestType type,string eventName, IEnumerable<T> item)
        {
            List<GenericEvent> events = new();

            if (type == RequestType.StockUpdated)
            {
                StockChangedEvent invAll = GetStockChengedEvent(eventName, item);
                events.Add(invAll);
            }

            return events;
        }
        private StockChangedEvent GetStockChengedEvent<T>(string eventName, IEnumerable<T> item)
        {
            return new StockChangedEvent()
            {
                EventName = eventName,
                RoutingKey = "stock",
                Changes = _mapper.Map<IEnumerable<StockChange>>(item)
            };
        }
    }
}
