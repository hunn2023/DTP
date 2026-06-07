using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Queries.Customers
{
    public class GetMyCustomerProfileQuery : IRequest<CustomerDto?>
    {
        public Guid UserId { get; set; }
    }

    public class GetMyCustomerProfileQueryHandler
       : IRequestHandler<GetMyCustomerProfileQuery, CustomerDto?>
    {
        private readonly ICustomerService _customerService;

        public GetMyCustomerProfileQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<CustomerDto?> Handle(
            GetMyCustomerProfileQuery request,
            CancellationToken cancellationToken)
        {
            return await _customerService.GetByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
