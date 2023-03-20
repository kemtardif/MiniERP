using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Services
{
    public class UnitService : IUnitService
    {
        private readonly ILogger<UnitService> _logger;
        private readonly IUnitRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<Unit> _validator;

        public UnitService(ILogger<UnitService> logger,
                           IUnitRepository repository,
                           IMapper mapper,
                           IValidator<Unit> validator)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }
        public Result<UnitReadDto> CreateUnit(UnitWriteDto dto)
        {
            Unit unit = _mapper.Map<Unit>(dto);

            ValidationResult validationResult = _validator.Validate(unit);

            if (!validationResult.IsValid)
            {
                return Result<UnitReadDto>.Failure(validationResult.ToDictionary());
            }

            _repository.AddUnit(unit);

            _repository.SaveChanges();

            _logger.LogInformation("Unit Created : Id = {id}, Date = {date}", unit.Id, DateTime.UtcNow);


            UnitReadDto unitReadDto = _mapper.Map<UnitReadDto>(unit);

            return Result<UnitReadDto>.Success(unitReadDto);
        }

        public Result<IEnumerable<UnitReadDto>> GetAllUnits()
        {
            IEnumerable<Unit> units = _repository.GetAllUnits();
            IEnumerable<UnitReadDto> unitDtos = _mapper.Map<IEnumerable<UnitReadDto>>(units);

            return Result<IEnumerable<UnitReadDto>>.Success(unitDtos);
        }
        public Result<UnitReadDto> GetUnitById(int id)
        {
            Unit? unit = _repository.GetUnitById(id);
            if (unit is null)
            {
                return Result<UnitReadDto>.Failure(GetNotFoundResult(id));
            }

            UnitReadDto readDto = _mapper.Map<UnitReadDto>(unit);

            return Result<UnitReadDto>.Success(readDto);
        }
        private IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(Unit)] = new string[] { $"Unit not found : ID = {id}" }
            };
        }
    }
}
