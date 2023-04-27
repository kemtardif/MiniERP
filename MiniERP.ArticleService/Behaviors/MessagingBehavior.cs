using MediatR;
using MiniERP.ArticleService.MessageBus.Sender.Contracts;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Behaviors
{
    public class MessagingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                                                    where TRequest : IRequest<TResponse>
                                                    where TResponse : Result
    {
        private readonly ILogger<MessagingBehavior<TRequest, TResponse>> _logger;
        private readonly IRabbitMQClient _rabbitMQClient;

        public MessagingBehavior(ILogger<MessagingBehavior<TRequest, TResponse>> logger,
                                 IRabbitMQClient rabbitMQClient)
        {
            _logger = logger;
            _rabbitMQClient = rabbitMQClient;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (response.HasMessage)
            {
                _rabbitMQClient.Publish(response.Message);

                _logger.LogInformation("Message published : {message} for request {request} and response {response}",
                                    response.Message.GetType().Name,
                                    typeof(TRequest).Name,
                                    typeof(TResponse).GetGenericArguments()[0].Name
                                    );
            }

            return response;
        }
    }
}
