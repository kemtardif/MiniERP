using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleReadDto>();
            CreateMap<Article, ArticleCreateDto>();
            CreateMap<Article, ArticleUpdateDto>();

            CreateMap<ArticleCreateDto, Article>();
            CreateMap<ArticleUpdateDto, Article>();

            CreateMap<Article, ArticleCreateMessage>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.Id));
            CreateMap<Article, ArticleUpdateMessage>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.Id));
            CreateMap<Article, ArticleDeleteMessage>()
                .ForMember(dest => dest.Id,
                            opts => opts.MapFrom(src => src.Id));

            CreateMap<JsonPatchDocument<ArticleUpdateDto>, JsonPatchDocument<Article>>();
            CreateMap<Operation<ArticleUpdateDto>, Operation<Article>>();
        }
    }
}
