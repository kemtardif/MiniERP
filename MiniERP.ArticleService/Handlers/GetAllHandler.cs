using AutoMapper;
using MediatR;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Queries;

namespace MiniERP.ArticleService.Handlers
{
    public class GetAllHandler : HandlerBase, IRequestHandler<GetAllQuery, Result<IEnumerable<ReadDTO>>>
    {
        public GetAllHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        { }

        public Task<Result<IEnumerable<ReadDTO>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var items = _repository.GetAllArticles();

            var dtos = _mapper.Map<IEnumerable<ReadDTO>>(items);

            return Task.FromResult(Result<IEnumerable<ReadDTO>>.Success(dtos));
        }
    }
}
