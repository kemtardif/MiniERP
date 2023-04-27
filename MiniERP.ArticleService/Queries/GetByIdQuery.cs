using MediatR;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Queries
{
    public class GetByIdQuery : IRequest<Result<ReadDTO>>
    {
        public int Id { get; }

        public GetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
