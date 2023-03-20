using AutoMapper;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.MessageBus.Events;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.MessageBus
{
    public class RabbitMQArticleSender : IMessageBusSender<Article>
    {
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;

        public RabbitMQArticleSender(
                                     IMapper mapper, 
                                     IMessageBusClient messageBus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }
        public void RequestForPublish(RequestType type, ChangeType changeType, Article article)
        {
            string eventName = GetEventName(type);
            IEnumerable<GenericEvent> events = GetEvents(eventName, changeType, article);

            foreach(GenericEvent evnt in events)
            {
                _messageBus.PublishNewArticle(evnt);
            }
        }

        private string GetEventName(RequestType type)
        {
            return type switch
            {
                RequestType.Created => MessageBusEventType.ArticleCreated,
                RequestType.Deleted => MessageBusEventType.ArticleDeleted,
                RequestType.Updated => MessageBusEventType.ArticleUpdated,
                _ => throw new ArgumentException(nameof(RequestType)),
            };
        }
        private IEnumerable<GenericEvent> GetEvents(string eventName, ChangeType change, Article article)
        {
            List<GenericEvent> events = new();

            if(change == ChangeType.All)
            {
                InventoryEvent invAll = GetInventoryEvent(eventName, article);
                events.Add(invAll);
            }
            else if (change.HasFlag(ChangeType.Inventory))
            {
                InventoryEvent invAll = GetInventoryEvent(eventName, article);
                events.Add(invAll);
            }
            return events;
        }
        private InventoryEvent GetInventoryEvent(string eventName, Article article)
        {
            InventoryEvent inv = _mapper.Map<InventoryEvent>(article);
            inv.EventName = eventName;
            inv.RoutingKey = "inventory";
            return inv;
        }


    }
}
