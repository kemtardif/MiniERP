using AutoMapper;
using MiniERP.SalesOrderService.Dtos;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Profiles
{
    public class SalesOrderProfile : Profile
    {
        public SalesOrderProfile()
        {
            CreateMap<SalesOrder, SalesOrderReadDto>();
            CreateMap<SalesOrderCreateDto, SalesOrder>();
            CreateMap<SalesOrderUpdateDto, SalesOrder>();
        }
    }
}
