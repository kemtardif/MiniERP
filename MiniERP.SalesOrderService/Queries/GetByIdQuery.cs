using MediatR;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Queries
{
    public class GetByIdQuery : IRequest<Result<SOReadDTO>>
    {
        public int Id { get; }

        public GetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
