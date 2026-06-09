using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
namespace DTP.Modules.Catalog.Application.Queries.Countries
{
    public class GetPublicCountriesQuery
      : IRequest<Result<PagedResultDto<CountryDto>>>
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetPublicCountriesQueryHandler
        : IRequestHandler<GetPublicCountriesQuery, Result<PagedResultDto<CountryDto>>>
    {
        private readonly ICountryService _countryService;

        public GetPublicCountriesQueryHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Result<PagedResultDto<CountryDto>>> Handle(
            GetPublicCountriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _countryService.GetPublicAsync(
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
