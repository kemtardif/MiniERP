using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Extensions;
using MiniERP.InventoryService.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace MiniERP.InventoryService.Controllers;

[ApiController]
[Authorize(Roles = "ApplicationHTTPRequestInvSrv")]
[Route("/api/inv-srv/[controller]")]
public class StocksController : ControllerBase
{

    private readonly ILogger<StocksController> _logger;
    private readonly IInventoryRepository _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public StocksController(ILogger<StocksController> logger, 
        IInventoryRepository repository, 
        IMapper mapper,
        IDistributedCache cache)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }
    [HttpGet]
    public ActionResult<IEnumerable<StockReadDto>> GetAllItems()
    {
        List<StockReadDto> dtos = new();

        foreach (Stock model in _repository.GetAllItems())
        {
            dtos.Add(_mapper.Map<StockReadDto>(model));
        }
        return dtos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockReadDto>> GetItemById(int id)
    {
        Stock? stock;
        try
        {
            stock = await _cache.GetRecordAsync<Stock>(id.ToString());
        } 
        catch(DistributedCacheException ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }

        if(stock is not null)
        {
            return Ok(_mapper.Map<StockReadDto>(stock));
        }
        stock = _repository.GetItemById(id);
        if (stock is not null)
        {
            await _cache.SetRecordAsync(stock.Id.ToString(), stock);

            return Ok(_mapper.Map<StockReadDto>(stock));
        }
        return NotFound();
    }
}
