using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.MessageBus.Events;
using MiniERP.ArticleService.Models;
using System;

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
            _mapper = mapper;
            _messageBus = messageBus;
        }
        public void RequestForPublish(RequestType type, Article article, ChangeType changeType)
        {
            string eventName = GetEventName(type);
            IEnumerable<GenericEvent> events = GetEvents(eventName, article, changeType);

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
                _ => throw new ArgumentNullException(nameof(type)),
            };
        }
        private IEnumerable<GenericEvent> GetEvents(string eventName, Article article, ChangeType change)
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
