using AutoMapper;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IInventoryRepository _repository;
        private readonly IMapper _mapper;

        public InventoryService(ILogger<InventoryService> logger,
                                IInventoryRepository repository,
                                IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
        public Result<IEnumerable<StockReadDto>> GetAllStocks()
        {
            IEnumerable<InventoryItem> stocks = _repository.GetAllItems();
            IEnumerable<StockReadDto> stockDtos = _mapper.Map<IEnumerable<StockReadDto>>(stocks);

            return Result<IEnumerable<StockReadDto>>.Success(stockDtos);
        }

        public Result<StockReadDto> GetStockByArticleId(int articleId)
        {
            InventoryItem? stock = _repository.GetItemByArticleId(articleId);

            if(stock is null)
            {
                return Result<StockReadDto>.Failure(GetNotFoundResult(articleId));
            }

            StockReadDto readDto = _mapper.Map<StockReadDto>(stock);

            return Result<StockReadDto>.Success(readDto);
        }

        private IDictionary<string, string[]> GetNotFoundResult(int articleId)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(InventoryItem)] = new string[] { $"Stock not found : ArticleID = {articleId}" }
            };
        }
    }
}
