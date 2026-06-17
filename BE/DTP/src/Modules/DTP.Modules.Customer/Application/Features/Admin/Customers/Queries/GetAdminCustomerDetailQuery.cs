using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Customer.Application.Features.Admin.Customers.Queries
{
    public class GetAdminCustomerDetailQuery : IRequest<Result<AdminCustomerDetailDto>>
    {
        public Guid UserId { get; set; }
    }
}
