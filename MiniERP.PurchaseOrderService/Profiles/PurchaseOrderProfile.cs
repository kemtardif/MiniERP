using AutoMapper;
using MiniERP.PurchaseOrderService.Dtos;
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
            CreateMap<POCreateDto, PurchaseOrder>()
                .ForMember(dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details));
        }
    }
}
