using AutoMapper;
using MediatR;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Queries;

namespace MiniERP.ArticleService.Handlers
{
    public class GetByIdHandler : HandlerBase, IRequestHandler<GetByIdQuery, Result<ReadDTO>>
    {
        public GetByIdHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        { }
        public Task<Result<ReadDTO>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            Article? item = _repository.GetArticleById(request.Id);

            if(item is null)
            {
                return Task.FromResult(Result<ReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            var dto = _mapper.Map<ReadDTO>(item);

            return Task.FromResult(Result<ReadDTO>.Success(dto));
        }
    }
}
