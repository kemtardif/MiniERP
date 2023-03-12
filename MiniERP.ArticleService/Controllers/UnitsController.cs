using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Controllers
{
    [Route("api/catalogue/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly ILogger<UnitsController> _logger;
        private readonly IUnitRepository _repository;
        private readonly IMapper _mapper;

        public UnitsController(ILogger<UnitsController> logger, IUnitRepository repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult<UnitReadDto> GetAllUnitss()
        {
            IEnumerable<Unit> units = _repository.GetAllUnits();
            return Ok(_mapper.Map<IEnumerable<UnitReadDto>>(units));
        }

        [HttpGet("{id}", Name = nameof(GetUnitById))]
        public ActionResult<UnitReadDto> GetUnitById(int id)
        {
            Unit? unit = _repository.GetUnitById(id);
            if (unit is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UnitReadDto>(unit));
        }
        [HttpPost]
        public ActionResult<UnitReadDto> CreateUnit(UnitWriteDto writeDto)
        {
            Unit unit = _mapper.Map<Unit>(writeDto);
            unit.CreatedAt = unit.UpdatedAt = DateTime.UtcNow;

            _repository.AddUnit(unit);

            try
            {
                _repository.SaveChanges();
            }
            catch (SaveChangesException ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }

            UnitReadDto readDto = _mapper.Map<UnitReadDto>(unit);
            return CreatedAtRoute(nameof(GetUnitById), new { id = readDto.Id }, readDto);
        }
    }
}
