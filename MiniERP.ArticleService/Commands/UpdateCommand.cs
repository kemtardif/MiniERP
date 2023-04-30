using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Commands
{
    public class UpdateCommand : IRequest<Result<ReadDTO>>
    {
        public int Id { get; }
        public JsonPatchDocument<UpdateDTO> Item { get; }
        public UpdateCommand(int id, JsonPatchDocument<UpdateDTO> item)
        {
            Id = id;
            Item = item;
        }
    }
}
