using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ArticleRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddArticle(Article item)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.SetCreatedAdToCurrentTime();
            item.SetUpdatedAtToCurrentTime();

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
            item.SetUpdatedAtToCurrentTime();
            item.CloseArticle();
        }

        public bool SaveChanges()
        {
            return  _context.SaveChanges() >= 0;
        }
        public bool HasValidUnits(int id)
        {
            return _context.Units.Any(x => x.Id == id);
        }
        public Article UpdateArticle(Article item, JsonPatchDocument<ArticleUpdateDto> json)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if(json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }
            var articleToWrite = _mapper.Map<ArticleUpdateDto>(item);
            json.ApplyTo(articleToWrite);

            _mapper.Map(articleToWrite, item);

            item.SetUpdatedAtToCurrentTime();
            return item;
        }
        public ChangeType TrackChanges(Article item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            EntityEntry<Article>? entity = _context.ChangeTracker
                                            .Entries<Article>()
                                            .FirstOrDefault(x => x.Entity.Id == item.Id);
            ChangeType type = ChangeType.None;

            if (entity is null
                || entity.OriginalValues.ToObject() is not Article oldArticle
                || entity.CurrentValues.ToObject() is not Article newArticle)
            {
                return type;
            }
            type |= TrackChangesForInventory(oldArticle, newArticle);
            return type;

            
        }
        private ChangeType TrackChangesForInventory(Article oldArticle, Article newArticle)
        {
            ChangeType invType = ChangeType.None;

            InventoryRecord oldRecord = new(oldArticle);
            InventoryRecord newRecord = new(newArticle);

            if(oldRecord != newRecord)
            {
                invType |= ChangeType.Inventory;
            }
            return invType;
        }
    }
}
