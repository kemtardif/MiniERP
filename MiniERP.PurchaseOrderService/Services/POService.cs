using AutoMapper;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.Dtos;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Services
{
    public class POService : IPOService
    {
        private readonly IPORepository _repository;
        private readonly IMapper _mapper;

        public POService(IPORepository repository,
                         IMapper mapper)
        { 
            _repository = repository;
            _mapper = mapper;
        }
        public Result<POReadDto> CreatePurchaseOrder(POCreateDto createDto)
        {
            var po = _mapper.Map<PurchaseOrder>(createDto);

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
