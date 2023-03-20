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
        bool HasValidUnits(int id);
        Article UpdateArticle(Article item, JsonPatchDocument<ArticleUpdateDto> json);
        ChangeType TrackChanges(Article item);
    }

    [Flags]
    public enum ChangeType
    {
        None = 0,
        Inventory = 1 << 1,
        All = Inventory
    }
}
