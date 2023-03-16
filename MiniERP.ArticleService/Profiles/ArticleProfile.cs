using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleReadDto>();
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<Article, ArticleCreateDto>();
            CreateMap<Article, InventoryPublishDto>();
            CreateMap<ArticleUpdateDto, Article>();
            CreateMap<Article, ArticleUpdateDto>();

            CreateMap<JsonPatchDocument<ArticleUpdateDto>, JsonPatchDocument<Article>>();
            CreateMap<Operation<ArticleUpdateDto>, Operation<Article>>();
        }
    }
}
