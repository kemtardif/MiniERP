using AutoMapper;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Protos;

namespace MiniERP.SalesOrderService.Profiles
{
    public class SalesOrderProfile : Profile
    {
        public SalesOrderProfile()
        {
            CreateMap<SalesOrderDetail, SalesOrderDetailReadDto>();
            CreateMap<SalesOrder, SalesOrderReadDto>()
                .ForMember(dest => dest.Details,
                            opt => opt.MapFrom(s => s.Details));
            CreateMap<SalesOrderDetailCreateDto, SalesOrderDetail>();
            CreateMap<SalesOrderCreateDto, SalesOrder>()
                .ForMember(dest => dest.Details,
                        opt => opt.MapFrom(s => s.Details));
            CreateMap<SalesOrderUpdateDto, SalesOrder>();
            CreateMap<SalesOrder, SalesOrderUpdateDto>();
            CreateMap<StockModel, Stock>()
                .ForMember(dest => dest.InventoryId,
                            opt => opt.MapFrom(src => src.Id));
        }
    }
}
