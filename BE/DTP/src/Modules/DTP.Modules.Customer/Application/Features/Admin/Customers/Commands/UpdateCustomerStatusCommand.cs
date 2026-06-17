using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Customer.Application.Features.Admin.Customers.Commands
{
    public class UpdateCustomerStatusCommand : IRequest<Result<CustomerStatusResultDto>>
    {
        public Guid UserId { get; set; }

        public bool IsActive { get; set; }

        public string? Reason { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }
    }
}
