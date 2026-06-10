using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.Countries
{
    public sealed record GetHomeCountriesQuery(
      string? Region,
      string? Keyword,
      int PageIndex = 1,
      int PageSize = 12
  ) : IRequest<Result<PagedResultDto<HomeCountryEsimDto>>>;


    public sealed class GetHomeCountriesQueryHandler
    : IRequestHandler<GetHomeCountriesQuery, Result<PagedResultDto<HomeCountryEsimDto>>>
    {
        private readonly ICountryService _countryService;

        public GetHomeCountriesQueryHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Result<PagedResultDto<HomeCountryEsimDto>>> Handle(
            GetHomeCountriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _countryService.GetHomeCountriesAsync(
                request.Region,
                request.Keyword,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
