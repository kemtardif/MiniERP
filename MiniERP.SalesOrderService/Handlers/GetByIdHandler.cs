using AutoMapper;
using MediatR;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Queries;

namespace MiniERP.SalesOrderService.Handlers
{
    public class GetByIdHandler : HandlerBase, IRequestHandler<GetByIdQuery, Result<SOReadDTO>>
    {
        public GetByIdHandler(IRepository repository, IMapper mapper) : base(repository, mapper) 
        {
        }

        public Task<Result<SOReadDTO>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            SalesOrder? so = _repository.GetSOById(request.Id);

            if (so is null)
            {
                return Task.FromResult(Result<SOReadDTO>.Failure(GetNotFoundResult(request.Id)));
            }

            var dto = _mapper.Map<SOReadDTO>(so);

            return Task.FromResult(Result<SOReadDTO>.Success(dto));
        }

        private IDictionary<string, string[]> GetNotFoundResult(int id)
        {
            return new Dictionary<string, string[]>
            {
                [nameof(SalesOrder)] = new string[] { $"Sales Order not found : ID = {id}" }
            };
        }
    }
}
