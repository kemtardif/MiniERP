using AutoMapper;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Profiles
{
    public class SalesOrderProfile : Profile
    {
        public SalesOrderProfile()
        {
            CreateMap<SalesOrderDetail, SODetailReadDTO>();
            CreateMap<SalesOrder, SOReadDTO>()
                .ForMember(dest => dest.Details,
                            opt => opt.MapFrom(s => s.Details));

            CreateMap<SODetailCreateDTO, SalesOrderDetail>();

            CreateMap<SOCreateDTO, SalesOrder>()
                .ForMember(dest => dest.Details,
                        opt => opt.MapFrom(s => s.Details))
                .ForMember(dest => dest.Status,
                        opt => opt.MapFrom(src => SalesOrderStatus.Open));

            CreateMap<UpdateSalesOrder, SalesOrder>();
            CreateMap<SalesOrder, UpdateSalesOrder>();

        }
    }
}
