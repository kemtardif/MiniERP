using MediatR;

namespace MiniERP.SalesOrderService.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
                                                                where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing request : {type}", typeof(TRequest));

            TResponse response = await next();

            _logger.LogInformation("Processed request : {request} with result : {response}", 
                                    typeof(TRequest).Name,
                                    typeof(TResponse).GetGenericArguments()[0]);
            return response;
        }
    }

}
