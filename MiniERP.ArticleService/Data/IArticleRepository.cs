using MiniERP.ArticleService.Models;
using System.Net.Http.Headers;

namespace MiniERP.ArticleService.Data
{
    public interface IArticleRepository
    {
        bool SaveChanges();
        IEnumerable<Article> GetAllArticles();
        Article? GetArticleById(int id);
        void AddArticle(Article item);
        void RemoveArticle(Article item);
        bool HasValidUnits(int id);
    }
}
