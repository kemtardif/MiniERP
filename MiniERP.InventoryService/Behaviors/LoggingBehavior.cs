using MediatR;

namespace MiniERP.InventoryService.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                                            where TRequest : IRequest<TResponse>
    {
        private const string ProcessingLogFormat = "Processing request {req}";
        private const string ProcessedLogFormat = "Processed request {req} with response {resp}";

        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation(ProcessingLogFormat, typeof(TRequest).Name);

            var response = await next();

            _logger.LogInformation(ProcessedLogFormat,
                                    typeof(TRequest).Name,
                                    typeof(TResponse).GetGenericArguments()[0].Name);
            return response;
        }
    }
}
