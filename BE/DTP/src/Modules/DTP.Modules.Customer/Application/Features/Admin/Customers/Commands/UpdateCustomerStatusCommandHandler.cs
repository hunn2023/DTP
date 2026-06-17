using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Customer.Application.Features.Admin.Customers.Commands
{
    public class UpdateCustomerStatusCommandHandler
        : IRequestHandler<UpdateCustomerStatusCommand, Result<CustomerStatusResultDto>>
    {
        private readonly IAdminCustomerService _service;

        public UpdateCustomerStatusCommandHandler(IAdminCustomerService service)
        {
            _service = service;
        }

        public async Task<Result<CustomerStatusResultDto>> Handle(
            UpdateCustomerStatusCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.UpdateStatusAsync(
                request.UserId,
                request.IsActive,
                request.Reason,
                request.UpdatedByUserId,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }
    }
}
