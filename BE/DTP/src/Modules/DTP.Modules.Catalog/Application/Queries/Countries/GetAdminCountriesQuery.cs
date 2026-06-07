using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Countries
{
    public class GetAdminCountriesQuery
      : IRequest<Result<PagedResultDto<CountryDto>>>
    {
        public string? Keyword { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
    public class GetAdminCountriesQueryHandler
        : IRequestHandler<GetAdminCountriesQuery, Result<PagedResultDto<CountryDto>>>
    {
        private readonly ICountryService _countryService;

        public GetAdminCountriesQueryHandler(
            ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Result<PagedResultDto<CountryDto>>> Handle(
            GetAdminCountriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _countryService.GetPagedAsync(
                request.Keyword,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
