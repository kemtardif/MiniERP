using MediatR;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Queries
{
    public class GetAllQuery : IRequest<Result<IEnumerable<ReadDTO>>>
    {
    }
}
