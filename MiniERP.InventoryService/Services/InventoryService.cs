using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public InventoryService(ILogger<InventoryService> logger,
                                IRepository repository,
                                IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
        public Result<IEnumerable<InventoryReadDTO>> GetAllInventories()
        {
            IEnumerable<InventoryItem> stocks = _repository.GetAllItems();
            IEnumerable<InventoryReadDTO> stockDtos = _mapper.Map<IEnumerable<InventoryReadDTO>>(stocks);

            return Result<IEnumerable<InventoryReadDTO>>.Success(stockDtos);
        }

        public Result<InventoryReadDTO> GetInventoryByArticleId(int articleId)
        {
            InventoryItem? stock = _repository.GetInventoryByArticleId(articleId);

            if(stock is null)
            {
                return Result<InventoryReadDTO>.Failure(GetNotFoundResult(articleId));
            }

            InventoryReadDTO readDto = _mapper.Map<InventoryReadDTO>(stock);

            return Result<InventoryReadDTO>.Success(readDto);
        }

        private IDictionary<string, string[]> GetNotFoundResult(int articleId)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(InventoryItem)] = new string[] { $"Inventory Item not found : ArticleID = {articleId}" }
            };
        }
    }
}
