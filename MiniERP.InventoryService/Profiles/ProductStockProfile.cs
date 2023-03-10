using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Profiles
{
    public class ProductStockProfile : Profile
    {
        public ProductStockProfile()
        {
            CreateMap<Stock, StockReadDto>();
        }
    }
}
