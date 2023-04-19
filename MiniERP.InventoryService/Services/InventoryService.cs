﻿using AutoMapper;
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
        public Result<IEnumerable<InventoryItemReadDto>> GetAllInventories()
        {
            IEnumerable<InventoryItem> stocks = _repository.GetAllItems();
            IEnumerable<InventoryItemReadDto> stockDtos = _mapper.Map<IEnumerable<InventoryItemReadDto>>(stocks);

            return Result<IEnumerable<InventoryItemReadDto>>.Success(stockDtos);
        }

        public Result<InventoryItemReadDto> GetInventoryByArticleId(int articleId)
        {
            InventoryItem? stock = _repository.GetInventoryByArticleId(articleId);

            if(stock is null)
            {
                return Result<InventoryItemReadDto>.Failure(GetNotFoundResult(articleId));
            }

            InventoryItemReadDto readDto = _mapper.Map<InventoryItemReadDto>(stock);

            return Result<InventoryItemReadDto>.Success(readDto);
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
