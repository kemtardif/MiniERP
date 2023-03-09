using Microsoft.EntityFrameworkCore;
using MiniERP.ArticleService.Exceptions;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;

        public ArticleRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddArticle(Article item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _context.Articles.Add(item);
        }

        public IEnumerable<Article> GetAllArticles()
        {
            return _context.Articles.ToList();
        }

        public Article? GetArticleById(int id)
        {
            return _context.Articles.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveArticle(Article item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Status = ArticleStatus.Closed;
        }

        public bool SaveChanges()
        {
            try
            {
               return  _context.SaveChanges() >= 0;
            } 
            catch(DbUpdateException updEx)
            {
                throw new SaveChangesException(typeof(Article), updEx.Message);
            }
        }
        public bool HasValidUnits(int id)
        {
            return _context.Units.Any(x => x.Id == id);
        }
    }
}
