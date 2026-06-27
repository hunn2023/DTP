using DTP.Modules.Customer.Application.Abstractions.Repositories;
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

    public class GetAdminCustomersQueryHandler
       : IRequestHandler<GetAdminCustomersQuery, Result<PagedResult<AdminCustomerListItemDto>>>
    {
        private readonly IAdminCustomerRepository _repository;

        public GetAdminCustomersQueryHandler(IAdminCustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<AdminCustomerListItemDto>>> Handle(
            GetAdminCustomersQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _repository.GetPagedAsync(
                request.Keyword,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);

            return Result<PagedResult<AdminCustomerListItemDto>>.Success(result);
        }
    }
}
