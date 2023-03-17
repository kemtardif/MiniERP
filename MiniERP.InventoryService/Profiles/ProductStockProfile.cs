using AutoMapper;
using MiniERP.InventoryService.Dtos;
using MiniERP.InventoryService.MessageBus.Responses;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Profiles
{
    public class ProductStockProfile : Profile
    {
        public ProductStockProfile()
        {
            CreateMap<Stock, StockReadDto>();
            CreateMap<ArticleResponse, Stock>()
                .ForSourceMember(x => x.EventName, opts => opts.DoNotValidate())
                .ForMember(dest => dest.ProductId,
                            opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Discontinued,
                            opt => opt.MapFrom<StatusToDiscontinuedResolver>());
        }
    }
    public class StatusToDiscontinuedResolver : IValueResolver<ArticleResponse, Stock, bool>
    {

        public bool Resolve(ArticleResponse source, Stock destination, bool destMember, ResolutionContext context)
        {
            return source.Status == 2;
        }
    }
}
