
using DTP.Modules.Customer.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Customer.Application.Commands.CustomerAddresses
{
    public class UpdateCustomerAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string ReceiverName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? CountryCode { get; set; }
    }

    public class UpdateCustomerAddressCommandHandler : IRequestHandler<UpdateCustomerAddressCommand, bool>
    {
        private readonly ICustomerAddressService _addressService;

        public UpdateCustomerAddressCommandHandler(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<bool> Handle(
            UpdateCustomerAddressCommand request,
            CancellationToken cancellationToken)
        {
            return await _addressService.UpdateAsync(request, cancellationToken);
        }
    }
}
