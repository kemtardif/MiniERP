using MediatR;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Commands
{
    public class DeleteCommand : IRequest<Result<ReadDTO>>
    {
        public int Id { get; }
        public DeleteCommand(int id)
        {
            Id = id;
        }
    }
}
