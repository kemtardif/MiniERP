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
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context == null)
            {
                _logger.LogCritical("---> {name} : {iexcetion} error : {date}",
                                    nameof(ErrorsController),
                                    nameof(IExceptionHandlerFeature),
                                    DateTime.UtcNow);
                return new ErrorResponse("Critical internal error");
            }

            Exception exception = context.Error;


            int code = exception switch
            {
                JsonPatchException json => (int)HttpStatusCode.UnprocessableEntity,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            _logger.LogError("Exception : {ex}: {stack} : {date}",
                                      exception.Message,
                                      exception.StackTrace,
                                      DateTime.UtcNow);

            HttpContext.Response.StatusCode = code;

            return new ErrorResponse(exception.Message);
        }
    }
}
