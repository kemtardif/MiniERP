using AutoMapper;
using MediatR;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Handlers
{
    public class DeleteHandler : HandlerBase, IRequestHandler<DeleteCommand, Result<ReadDTO>>
    {
        public DeleteHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        { }

        public Task<Result<ReadDTO>> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var item = _repository.GetArticleById(request.Id);

            if(item is null)
            {
                return Task.FromResult(Result<ReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            _repository.RemoveArticle(item);
            _repository.SaveChanges();

            var dto = _mapper.Map<ReadDTO>(item);

            var message = _mapper.Map<ArticleDeleteMessage>(item);

            return Task.FromResult(Result<ReadDTO>.Success(dto, message));
        }
    }
}
