using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Commands.Customers
{
    public class CreateCustomerCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerService _customerService;

        public CreateCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<Guid> Handle(
            CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            return await _customerService.CreateAsync(request, cancellationToken);
        }
    }
}
