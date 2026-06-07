using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Commands.Customers
{
    public class ChangeCustomerStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public CustomerStatus Status { get; set; }
    }

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomerService _customerService;

        public DeleteCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<bool> Handle(
            DeleteCustomerCommand request,
            CancellationToken cancellationToken)
        {
            return await _customerService.DeleteAsync(request, cancellationToken);
        }
    }
}
