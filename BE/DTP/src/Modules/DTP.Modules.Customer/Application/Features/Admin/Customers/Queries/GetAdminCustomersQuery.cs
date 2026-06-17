using DTP.Modules.Customer.Application.Common;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Customer.Application.Features.Admin.Customers.Queries
{
    public class GetAdminCustomersQuery : IRequest<Result<PagedResult<AdminCustomerListItemDto>>>
    {
        public string? Keyword { get; set; }

        public bool? IsActive { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
