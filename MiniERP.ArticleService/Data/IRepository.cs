using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public interface IRepository
    {
        bool SaveChanges();
        IEnumerable<Article> GetAllArticles();
        Article? GetArticleById(int id);
        void AddArticle(Article item);
        void  RemoveArticle(Article item);
        Article UpdateArticle(Article item, JsonPatchDocument<UpdateDTO> json);
    }
}
