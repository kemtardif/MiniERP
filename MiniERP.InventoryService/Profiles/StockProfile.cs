using AutoMapper;
using MiniERP.InventoryService.DTOs;
using MiniERP.InventoryService.MessageBus.Messages;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Profiles
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<Stock, StockModel>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.ArticleId));

            CreateMap<Stock, StockReadDTO>();

        }
    }
}
