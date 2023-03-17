using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniERP.InventoryService.Models;
using System.Net;

namespace MiniERP.InventoryService.Controllers
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
                _logger.LogCritical("---> {name} : {excetion} error : {date}",
                                    nameof(ErrorsController),
                                    nameof(IExceptionHandlerFeature),
                                    DateTime.UtcNow);
                return new ErrorResponse("Critical internal error");
            }

            Exception exception = context.Error;


            int code = exception switch
            {
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
