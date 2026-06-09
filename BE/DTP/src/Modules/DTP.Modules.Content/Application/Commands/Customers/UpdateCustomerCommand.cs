
using DTP.Modules.Customer.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Customer.Application.Commands.Customers
{
    public class UpdateCustomerCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Note { get; set; }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly ICustomerService _customerService;

        public UpdateCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<bool> Handle(
            UpdateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            return await _customerService.UpdateAsync(request, cancellationToken);
        }
    }
}
