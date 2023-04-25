using AutoMapper;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Profiles
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<PODetail, PODetailReadDTO>();
            CreateMap<PurchaseOrder, POReadDTO>()
                .ForMember(dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details));

            CreateMap<PODetailCreateDTO, PODetail>();
            CreateMap<POCreateDTO, PurchaseOrder>()
                .ForMember(dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details));
        }
    }
}
