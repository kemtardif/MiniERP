using AutoMapper;
using MediatR;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Handlers
{
    public class UpdateHandler : HandlerBase, IRequestHandler<UpdateCommand, Result<ReadDTO>>
    {
        public UpdateHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        { }

        public Task<Result<ReadDTO>> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            var item = _repository.GetArticleById(request.Id);

            if (item is null)
            {
                return Task.FromResult(Result<ReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            _repository.UpdateArticle(item, request.Item);

            _repository.SaveChanges();

            var dto = _mapper.Map<ReadDTO>(item);

            var message = _mapper.Map<ArticleUpdateMessage>(item);

            return Task.FromResult(Result<ReadDTO>.Success(dto, message));
        }

        private IDictionary<string, string[]> GetCaughtExceptionResult(string message)
        {
            return new Dictionary<string, string[]>
            {
                ["message"] = new string[] { message }
            };
        }
    }
}
