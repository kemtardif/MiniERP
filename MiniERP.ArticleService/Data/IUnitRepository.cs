using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public interface IUnitRepository
    {
        bool SaveChanges();
        IEnumerable<Unit> GetAllUnits();
        Unit? GetUnitById(int id);
        void AddUnit(Unit item);
        void RemoveUnit(Unit item);
    }
}
