using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ReadDTO>();
            CreateMap<Article, CreateDTO>();
            CreateMap<Article, UpdateDTO>();

            CreateMap<CreateDTO, Article>();
            CreateMap<UpdateDTO, Article>();


            CreateMap<Article, ArticleCreateMessage>();
            CreateMap<Article, ArticleUpdateMessage>();
            CreateMap<Article, ArticleDeleteMessage>();

            CreateMap<JsonPatchDocument<UpdateDTO>, JsonPatchDocument<Article>>();
            CreateMap<Operation<UpdateDTO>, Operation<Article>>();
        }
    }
}
