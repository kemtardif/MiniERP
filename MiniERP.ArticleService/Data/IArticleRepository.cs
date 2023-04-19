using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public interface IArticleRepository
    {
        bool SaveChanges();
        IEnumerable<Article> GetAllArticles();
        Article? GetArticleById(int id);
        void AddArticle(Article item);
        void  RemoveArticle(Article item);
        Article UpdateArticle(Article item, JsonPatchDocument<ArticleUpdateDto> json);
    }
}
