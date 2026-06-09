
using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Queries.CustomerAddresses
{
    public class GetCustomerAddressesQuery : IRequest<List<CustomerAddressDto>>
    {
        public Guid CustomerId { get; set; }
    }

    public class GetCustomerAddressesQueryHandler
       : IRequestHandler<GetCustomerAddressesQuery, List<CustomerAddressDto>>
    {
        private readonly ICustomerAddressService _addressService;

        public GetCustomerAddressesQueryHandler(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<List<CustomerAddressDto>> Handle(
            GetCustomerAddressesQuery request,
            CancellationToken cancellationToken)
        {
            return await _addressService.GetByCustomerIdAsync(
                request.CustomerId,
                cancellationToken);
        }
    }
}
