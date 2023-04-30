using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.MessageBus.Sender.Contracts;
using MiniERP.InventoryService.Models;
using Quartz;

namespace MiniERP.InventoryService.Jobs
{
    [DisallowConcurrentExecution]
    public class AutoOrderJob : IJob
    {
        private readonly IStockSourcing _source;
        private readonly IRabbitMQClient _rabbitMQClient;
        private readonly ILogger<AutoOrderJob> _logger;

        public AutoOrderJob(IStockSourcing source,
                            IRabbitMQClient rabbitMQCLient,
                            ILogger<AutoOrderJob> logger)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _rabbitMQClient = rabbitMQCLient ?? throw new ArgumentNullException(nameof(rabbitMQCLient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("----------Starting Job {name}-----------", nameof(AutoOrderJob));

            List<Stock> orders = _source.GetStockToAutoOrder()
                                        .ToList();

            _logger.LogInformation("----------{name} : {count} items to AutoOrder-----------", 
                                    nameof(AutoOrderJob),
                                    orders.Count);

            foreach (Stock order in orders)
            {
                _rabbitMQClient.Publish(new POCreate(order.ArticleId, order.Quantity));
            }

            _logger.LogInformation("----------Ending Job {name}-----------", nameof(AutoOrderJob));
            return Task.CompletedTask;
        }
    }
}
