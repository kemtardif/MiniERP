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
            CreateMap<ArticleWriteDto, Article>();
            CreateMap<Article, ArticleWriteDto>();
            CreateMap<Article, InventoryPublishDto>();

            CreateMap<JsonPatchDocument<ArticleWriteDto>, JsonPatchDocument<Article>>();
            CreateMap<Operation<ArticleWriteDto>, Operation<Article>>();
        }
    }
}
