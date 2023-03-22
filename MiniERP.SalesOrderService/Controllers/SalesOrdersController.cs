using Microsoft.AspNetCore.Mvc;

namespace MiniERP.SalesOrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesOrdersController : ControllerBase
    {

        private readonly ILogger<SalesOrdersController> _logger;

        public SalesOrdersController(ILogger<SalesOrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Array.Empty<string>();
        }
    }
}