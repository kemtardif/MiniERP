using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Services
{
    public class POService : IPOService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<PurchaseOrder> _validator;

        public POService(IRepository repository,
                         IMapper mapper,
                         IValidator<PurchaseOrder> validator)
        { 
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }
        public Result<POReadDto> CreatePurchaseOrder(POCreateDTO createDto)
        {
            var po = _mapper.Map<PurchaseOrder>(createDto);

            ValidationResult validationResult= _validator.Validate(po);

            if(!validationResult.IsValid)
            {
                return Result<POReadDto>.Failure(validationResult.ToDictionary());
            }

            _repository.AddPO(po);

            _repository.SaveChanges();

            var dto = _mapper.Map<POReadDto>(po);

            return Result<POReadDto>.Success(dto);
        }

        public Result<IEnumerable<POReadDto>> GetAllPurchaseOrders()
        {
            IEnumerable<PurchaseOrder> orders = _repository.GetAllPOs();

            var dtos = _mapper.Map<IEnumerable<POReadDto>>(orders);

            return Result<IEnumerable<POReadDto>>.Success(dtos);
        }

        public Result<POReadDto> GetPOById(int id)
        {
            PurchaseOrder? po = _repository.GetPOById(id);

            if (po is null)
            {
                return Result<POReadDto>.Failure(GetNotFoundResult(id));
            }

            var dto = _mapper.Map<POReadDto>(po);

            return Result<POReadDto>.Success(dto);
        }

        #region Private Methods
        private IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(PurchaseOrder)] = new string[] { $"Purchase Order not found : ID = {id}" }
            };
        }
        #endregion
    }
}
