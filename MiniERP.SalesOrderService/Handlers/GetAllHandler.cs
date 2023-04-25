﻿using AutoMapper;
using MediatR;
using MiniERP.SalesOrderService.Data;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Queries;

namespace MiniERP.SalesOrderService.Handlers
{
    public class GetAllHandler : IRequestHandler<GetAllQuery, Result<IEnumerable<SOReadDTO>>>
    {
        private readonly ISalesOrderRepository _repository;
        private readonly IMapper _mapper;

        public GetAllHandler(ISalesOrderRepository repository,
                             IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<Result<IEnumerable<SOReadDTO>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<SalesOrder> sos = _repository.GetAllSalesOrders();

            var dtos = _mapper.Map<IEnumerable<SOReadDTO>>(sos);

            return Task.FromResult(Result<IEnumerable<SOReadDTO>>.Success(dtos));
        }
    }
}