using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniERP.PurchaseOrderService.Exceptions;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private const string CriticalLogFormat = "Critical error : {err} : {id} : {date}";
        private const string FriendlyLogFormat = "{friendly} : {message} : {id} : {date}";
        private const string InternalLogFormat = "Internal error : {id} : {date}";
        private const string CritialInternalError = "Critical internal error";

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
                _logger.LogCritical(CriticalLogFormat,
                                    nameof(IExceptionHandlerFeature),
                                    HttpContext.TraceIdentifier,
                                    DateTime.UtcNow);
                return new ErrorResponse(CritialInternalError);
            }

            Exception exception = context.Error;

            switch (exception)
            {
                case HttpFriendlyException friendly:
                    _logger.LogError(FriendlyLogFormat,
                                      exception.Message,
                                      exception.InnerException?.Message,
                                      HttpContext.TraceIdentifier,
                                      DateTime.UtcNow);
                    return new ErrorResponse(exception.Message);
                default:
                    _logger.LogError(exception, InternalLogFormat,
                                      HttpContext.TraceIdentifier,
                                      DateTime.UtcNow);
                    return new ErrorResponse(CritialInternalError);
            }
        }
    }
}
