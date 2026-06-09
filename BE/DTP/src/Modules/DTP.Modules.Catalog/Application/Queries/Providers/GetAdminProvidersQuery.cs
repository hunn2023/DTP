using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Providers
{
    public class GetAdminProvidersQuery
         : IRequest<Result<PagedResultDto<ProviderDto>>>
    {
        public string? Keyword { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetAdminProvidersQueryHandler
       : IRequestHandler<GetAdminProvidersQuery, Result<PagedResultDto<ProviderDto>>>
    {
        private readonly IProviderService _providerService;

        public GetAdminProvidersQueryHandler(IProviderService providerService)
        {
            _providerService = providerService;
        }

        public async Task<Result<PagedResultDto<ProviderDto>>> Handle(
            GetAdminProvidersQuery request,
            CancellationToken cancellationToken)
        {
            return await _providerService.GetPagedAsync(
                request.Keyword,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
