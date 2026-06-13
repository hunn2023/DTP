using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Domain.Enums;
using MediatR;

namespace DTP.Modules.Customer.Application.Commands.Customers
{
    public class ChangeCustomerStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public CustomerStatus Status { get; set; }
    }

    public class ChangeCustomerStatusCommandHandler : IRequestHandler<ChangeCustomerStatusCommand, bool>
    {
        private readonly ICustomerService _customerService;

        public ChangeCustomerStatusCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<bool> Handle(
            ChangeCustomerStatusCommand request,
            CancellationToken cancellationToken)
        {
            return await _customerService.ChangeStatusAsync(request, cancellationToken);
        }
    }
}
