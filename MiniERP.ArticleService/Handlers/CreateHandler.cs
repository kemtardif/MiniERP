using AutoMapper;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.MessageBus.Messages;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Handlers
{
    public class CreateHandler : HandlerBase<CreateCommand, Result<ReadDTO>>
    {
        public CreateHandler(IRepository repository, IMapper mapper) : base(repository, mapper){ }

        public override Task<Result<ReadDTO>> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var item = _mapper.Map<Article>(request.Item);

            _repository.AddArticle(item);
            _repository.SaveChanges();

            var dto = _mapper.Map<ReadDTO>(item);

            var message = _mapper.Map<ArticleCreateMessage>(item);

            return Task.FromResult(Result<ReadDTO>.Success(dto , message));

        }
    }
}
