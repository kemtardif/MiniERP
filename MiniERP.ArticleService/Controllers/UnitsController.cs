using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Controllers
{

    [Authorize(Roles = "ApplicationHTTPRequestArtSrv")]
    [ApiController]
    [Route("api/art-srv/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly ILogger<UnitsController> _logger;
        private readonly IUnitRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<Unit> _validator;

        public UnitsController(ILogger<UnitsController> logger, 
                                IUnitRepository repository, 
                                IMapper mapper,
                                IValidator<Unit> validator)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
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

            if(!Validate(unit))
            {
                return UnprocessableEntity(ModelState);
            }

            _repository.AddUnit(unit);
            _repository.SaveChanges();

            UnitReadDto readDto = _mapper.Map<UnitReadDto>(unit);
            return CreatedAtRoute(nameof(GetUnitById), new { id = readDto.Id }, readDto);
        }
        private bool Validate(Unit unit)
        {
            ValidationResult result = _validator.Validate(unit);
            if (result.IsValid)
            {
                return true;
            }
            foreach (ValidationFailure failure in result.Errors)
            {
                ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
            return false;
        }
    }
}
