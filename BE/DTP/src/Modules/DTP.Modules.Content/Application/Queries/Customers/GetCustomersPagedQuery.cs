using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Queries.Customers
{
    public class GetCustomersPagedQuery : IRequest<PagedResultDto<CustomerListItemDto>>
    {
        public string? Keyword { get; set; }

        public CustomerStatus? Status { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetCustomersPagedQueryHandler
       : IRequestHandler<GetCustomersPagedQuery, PagedResultDto<CustomerListItemDto>>
    {
        private readonly ICustomerService _customerService;

        public GetCustomersPagedQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<PagedResultDto<CustomerListItemDto>> Handle(
            GetCustomersPagedQuery request,
            CancellationToken cancellationToken)
        {
            return await _customerService.GetPagedAsync(
                request.Keyword,
                request.Status,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }

}
