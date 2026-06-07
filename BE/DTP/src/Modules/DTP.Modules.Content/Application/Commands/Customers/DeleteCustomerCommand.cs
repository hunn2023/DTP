using DTP.Modules.Customer.Domain.Enums;
using DTP.Modules.Customer.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Customer.Application.Commands.Customers
{
    public class DeleteCustomerCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

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
