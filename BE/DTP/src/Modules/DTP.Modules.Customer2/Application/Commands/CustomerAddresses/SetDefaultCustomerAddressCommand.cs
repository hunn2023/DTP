

using DTP.Modules.Customer.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Customer.Application.Commands.CustomerAddresses
{
    public class SetDefaultCustomerAddressCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }

        public Guid AddressId { get; set; }
    }


    public class SetDefaultCustomerAddressCommandHandler : IRequestHandler<SetDefaultCustomerAddressCommand, bool>
    {
        private readonly ICustomerAddressService _addressService;

        public SetDefaultCustomerAddressCommandHandler(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<bool> Handle(
            SetDefaultCustomerAddressCommand request,
            CancellationToken cancellationToken)
        {
            return await _addressService.SetDefaultAsync(request, cancellationToken);
        }
    }
}
