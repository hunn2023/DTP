
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
    public class GetDefaultCustomerAddressQuery : IRequest<CustomerAddressDto?>
    {
        public Guid CustomerId { get; set; }
    }


    public class GetDefaultCustomerAddressQueryHandler
      : IRequestHandler<GetDefaultCustomerAddressQuery, CustomerAddressDto?>
    {
        private readonly ICustomerAddressService _addressService;

        public GetDefaultCustomerAddressQueryHandler(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<CustomerAddressDto?> Handle(
            GetDefaultCustomerAddressQuery request,
            CancellationToken cancellationToken)
        {
            return await _addressService.GetDefaultAsync(
                request.CustomerId,
                cancellationToken);
        }
    }
}
