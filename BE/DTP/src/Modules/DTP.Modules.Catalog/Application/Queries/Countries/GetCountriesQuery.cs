using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Countries
{
    public class GetCountriesQuery : IRequest<List<CountryDto>>
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetCountriesQueryhandler
     : IRequestHandler<GetCountriesQuery, List<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;

        public GetCountriesQueryhandler(
            ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<CountryDto>> Handle(
            GetCountriesQuery request,
            CancellationToken cancellationToken)
        {
            var countries = await _countryRepository.GetActiveListAsync(
                cancellationToken);

            return countries.Select(x => new CountryDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Slug = x.Slug,
                FlagUrl = x.FlagUrl,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            }).ToList();
        }
    }
}
