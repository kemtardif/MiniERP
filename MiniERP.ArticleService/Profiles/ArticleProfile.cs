﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.MessageBus.Events;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Protos;

namespace MiniERP.ArticleService.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleReadDto>();
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<Article, ArticleCreateDto>();
            CreateMap<Article, InventoryEvent>();
            CreateMap<ArticleUpdateDto, Article>();
            CreateMap<Article, ArticleUpdateDto>();
            CreateMap<Article, GrpcInventoryItemModel>();

            CreateMap<JsonPatchDocument<ArticleUpdateDto>, JsonPatchDocument<Article>>();
            CreateMap<Operation<ArticleUpdateDto>, Operation<Article>>();
        }
    }
}
