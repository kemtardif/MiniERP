using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Handlers
{
    public class UpdateHandler : IRequestHandler<UpdateCommand, Result<SOReadDTO>>
    {
        public Task<Result<SOReadDTO>> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
