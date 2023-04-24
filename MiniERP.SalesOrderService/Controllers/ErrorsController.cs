﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniERP.SalesOrderService.Exceptions;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;
        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }
        [Route("error")]
        public ErrorResponse Error()
        {
            HttpContext.Response.StatusCode = 500;

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context is null)
            {
                _logger.LogCritical("Critical error : {err} : {id} : {date}",
                                    nameof(IExceptionHandlerFeature),
                                    HttpContext.TraceIdentifier,
                                    DateTime.UtcNow);
                return new ErrorResponse("Critical internal error");
            }

            Exception exception = context.Error;

            switch (exception)
            {
                case HttpFriendlyException friendly:
                    _logger.LogError(exception, "{friendly} : {message} : {id} : {date}",
                                      exception.Message,
                                      exception.InnerException?.Message,
                                      HttpContext.TraceIdentifier,
                                      DateTime.UtcNow);
                    return new ErrorResponse(exception.Message);
                default:
                    _logger.LogError(exception, "Internal error : {id} : {date}",
                                      HttpContext.TraceIdentifier,
                                      DateTime.UtcNow);
                    return new ErrorResponse("Critical internal error");
            }
        }
    }
}
