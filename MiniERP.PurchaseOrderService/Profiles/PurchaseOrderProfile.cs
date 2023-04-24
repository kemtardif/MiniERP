using AutoMapper;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Profiles
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<PODetail, PODetailReadDto>();
            CreateMap<PurchaseOrder, POReadDto>()
                .ForMember(dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details));

            CreateMap<PODetailCreateDto, PODetail>();
            CreateMap<POCreateDTO, PurchaseOrder>()
                .ForMember(dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details));
        }
    }
}
