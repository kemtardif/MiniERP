using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Services;

namespace MiniERP.ArticleService.Controllers
{

    [Authorize(Roles = "ApplicationHTTPRequestArtSrv")]
    [ApiController]
    [Route("api/art-srv/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly ILogger<UnitsController> _logger;
        private readonly IUnitService _unitService;

        public UnitsController(ILogger<UnitsController> logger, 
                                IUnitService unitService)
        {
            _logger = logger;
            _unitService = unitService;
        }
        [HttpGet]
        public ActionResult<UnitReadDto> GetAllUnits()
        {
            try
            {
                Result<IEnumerable<UnitReadDto>> result = _unitService.GetAllUnits();

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError("{id} : An error occured while getting all units : {error} : {date}",
                    HttpContext.TraceIdentifier,
                    ex.Message,
                    DateTime.UtcNow);
                throw new HttpFriendlyException("An error occured while getting all articles", ex);
            }
        }

        [HttpGet("{id}", Name = nameof(GetUnitById))]
        public ActionResult<UnitReadDto> GetUnitById(int id)
        {
            try
            {
                Result<UnitReadDto> result = _unitService.GetUnitById(id);
                if (!result.IsSuccess)
                {
                    return NotFound();
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError("{id} : An error occured while getting all units : {error} : {date}",
                    HttpContext.TraceIdentifier,
                    ex.Message,
                    DateTime.UtcNow);
                throw new HttpFriendlyException("An error occured while getting all articles", ex);
            }
        }
        [HttpPost]
        public ActionResult<UnitReadDto> CreateUnit(UnitWriteDto writeDto)
        {
            try
            {
                Result<UnitReadDto> result = _unitService.CreateUnit(writeDto);
                if (!result.IsSuccess)
                {
                    return UnprocessableEntity(result.Errors);
                }

                return CreatedAtRoute(nameof(GetUnitById), new { id = result.Value.Id }, result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError("{id} : An error occured while getting all units : {error} : {date}",
                    HttpContext.TraceIdentifier,
                    ex.Message,
                    DateTime.UtcNow);
                throw new HttpFriendlyException("An error occured while getting all articles", ex);
            }
        }
    }
}
