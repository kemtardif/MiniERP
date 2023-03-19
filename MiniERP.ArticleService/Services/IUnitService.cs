using MiniERP.ArticleService.Dtos;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Services
{
    public interface IUnitService
    {
        Result<IEnumerable<UnitReadDto>> GetAllUnits();
        Result<UnitReadDto> GetUnitById(int id);
        Result<UnitReadDto> CreateUnit(UnitWriteDto dto);
    }
}
