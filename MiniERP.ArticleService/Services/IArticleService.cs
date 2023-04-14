using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Services
{
    public interface IArticleService
    {
        Result<IEnumerable<ArticleReadDto>> GetAllArticles();
        Result<ArticleReadDto> GetArticleById(int articleId);
        Result<ArticleReadDto> CreateArticle(ArticleCreateDto dto);
        Result RemoveArticleById(int articleId);
        Result<ArticleReadDto> PatchArticle(int articleId, JsonPatchDocument<ArticleUpdateDto> document);
    }
}
