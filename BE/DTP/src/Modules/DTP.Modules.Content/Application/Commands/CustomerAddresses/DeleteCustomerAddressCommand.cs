using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Commands.CustomerAddresses
{
    public class DeleteCustomerAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCustomerAddressCommandHandler : IRequestHandler<DeleteCustomerAddressCommand, bool>
    {
        private readonly ICustomerAddressService _addressService;

        public DeleteCustomerAddressCommandHandler(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<bool> Handle(
            DeleteCustomerAddressCommand request,
            CancellationToken cancellationToken)
        {
            return await _addressService.DeleteAsync(request, cancellationToken);
        }
    }
}
