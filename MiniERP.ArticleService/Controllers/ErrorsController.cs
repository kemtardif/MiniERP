using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Models;
using System.Net;

namespace MiniERP.ArticleService.Controllers
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

            switch(exception)
            {
                case HttpFriendlyException friendly:
                    _logger.LogError("{friendly} : {message} : {id} : {date}",
                                      exception.Message,
                                      exception.InnerException?.Message,
                                      HttpContext.TraceIdentifier,
                                      DateTime.UtcNow);
                    return new ErrorResponse(exception.Message);
                default:
                    _logger.LogError("{ex} : {id} : {date}",
                                      exception.Message,
                                      HttpContext.TraceIdentifier,
                                      DateTime.UtcNow);
                    return new ErrorResponse("Critical internal error");
            }
        }
    }
}
