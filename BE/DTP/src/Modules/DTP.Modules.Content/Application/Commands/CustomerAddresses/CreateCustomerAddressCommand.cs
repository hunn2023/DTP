using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Commands.CustomerAddresses
{
    public class CreateCustomerAddressCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }

        public string ReceiverName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? CountryCode { get; set; }

        public bool IsDefault { get; set; }
    }

    public class CreateCustomerAddressCommandHandler : IRequestHandler<CreateCustomerAddressCommand, Guid>
    {
        private readonly ICustomerAddressService _addressService;

        public CreateCustomerAddressCommandHandler(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<Guid> Handle(
            CreateCustomerAddressCommand request,
            CancellationToken cancellationToken)
        {
            return await _addressService.CreateAsync(request, cancellationToken);
        }
    }
}
