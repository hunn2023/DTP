using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Queries.Customers
{
    public class GetCustomerDetailQuery : IRequest<CustomerDetailDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerDetailQueryHandler
      : IRequestHandler<GetCustomerDetailQuery, CustomerDetailDto?>
    {
        private readonly ICustomerService _customerService;

        public GetCustomerDetailQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<CustomerDetailDto?> Handle(
            GetCustomerDetailQuery request,
            CancellationToken cancellationToken)
        {
            return await _customerService.GetDetailAsync(request.Id, cancellationToken);
        }
    }
}
