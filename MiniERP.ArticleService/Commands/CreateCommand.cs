using MediatR;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Commands
{
    public class CreateCommand : IRequest<Result<ReadDTO>>
    {
        public CreateDTO Item { get; }
        public CreateCommand(CreateDTO item)
        {
            Item = item;
        }
    }
}
