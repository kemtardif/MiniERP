using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Controllers;

[ApiController]
[Route("[controller]")]
public class StocksController : ControllerBase
{

    private readonly ILogger<StocksController> _logger;
    private readonly IInventoryRepository _repository;
    private readonly IMapper _mapper;

    public StocksController(ILogger<StocksController> logger, IInventoryRepository repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }
    [HttpGet]
    public ActionResult<IEnumerable<StockReadDto>> GetAllItems()
    {
        List<StockReadDto> dtos = new();

        foreach(Stock model in _repository.GetAllItems())
        {
            dtos.Add(_mapper.Map<StockReadDto>(model));
        }
        return dtos;
    }

    [HttpGet("id/{id}")]
    public ActionResult<StockReadDto> GetItemById(int id)
    {
        Stock? stock = _repository.GetItemById(id);
        if(stock is null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<StockReadDto>(stock));
    }
    [HttpGet("article/{id}")]
    public ActionResult<StockReadDto> GetItemByArticle(int id)
    {
        Stock? stock = _repository.GetItemByArticleId(id);
        if (stock is null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<StockReadDto>(stock));
    }
}
