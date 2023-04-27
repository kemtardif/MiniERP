﻿using MediatR;

namespace MiniERP.SalesOrderService.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
                                                                where TRequest : IRequest<TResponse>
    {
        private const string ProcessingLogFormat = "Processing request : {type}";
        private const string ProcessedLogFormat = "Processed request : {request} with result : {response}";

        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation(ProcessingLogFormat, typeof(TRequest));

            TResponse response = await next();

            _logger.LogInformation(ProcessedLogFormat, 
                                    typeof(TRequest).Name,
                                    typeof(TResponse).GetGenericArguments()[0]);
            return response;
        }
    }

}
