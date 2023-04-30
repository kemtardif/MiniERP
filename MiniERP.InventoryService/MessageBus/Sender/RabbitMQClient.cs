﻿using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.MessageBus.Sender.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace MiniERP.InventoryService.MessageBus.Sender
{
    public class RabbitMQClient : IRabbitMQClient, IDisposable
    {
        private const string PublishedLogFormat = "RabbitMQ : Purchase Order Message Published : {type} {date}";
        private const string PO_EXCHANGE = "purchaseorder";

        private readonly IModel _channel;
        private readonly ILogger<RabbitMQClient> _logger;
        public RabbitMQClient(IRabbitMQConnection rabbitMQConnection,
                              ILogger<RabbitMQClient> logger)
        {
            if(!rabbitMQConnection.Connection.IsOpen)
            {
                throw new BrokerUnreachableException(new ArgumentNullException(nameof(rabbitMQConnection)));
            }

            _channel = rabbitMQConnection.Connection.CreateModel();
            _logger = logger;

            _channel.ExchangeDeclare(exchange: PO_EXCHANGE, type: ExchangeType.Direct);
        }
        public void Publish(MessageBase message)
        {
            if(message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            string messageString = JsonSerializer.Serialize(message, message.GetType());

            byte[] messageByte = Encoding.UTF8.GetBytes(messageString);

            IBasicProperties properties = GetBasicProperties(message);

            _channel.BasicPublish(exchange: PO_EXCHANGE,
                                  routingKey: string.Empty,
                                  basicProperties: properties,
                                  body: messageByte);

            _logger.LogInformation(PublishedLogFormat,
                                    message.GetType(),
                                    DateTime.UtcNow);
        }

        private IBasicProperties GetBasicProperties(MessageBase message)
        {
            IBasicProperties properties = _channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>();

            foreach(var kvp in message.Headers)
            {
                properties.Headers.Add(kvp.Key, kvp.Value);
            }

            return properties;
        }

        public void Dispose()
        {
            _channel.Close();
        }
    }
}
