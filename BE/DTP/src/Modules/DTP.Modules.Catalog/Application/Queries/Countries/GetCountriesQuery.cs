using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Countries
{
    public class GetCountriesQuery : IRequest<Result<List<CountryDto>>>
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetCountriesQueryhandler
     : IRequestHandler<GetCountriesQuery, Result<List<CountryDto>>>
    {
        private readonly ICountryRepository _countryRepository;


        public GetCountriesQueryhandler(
            ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<Result<List<CountryDto>>> Handle(
            GetCountriesQuery request,
            CancellationToken cancellationToken)
        {
            var countries = await _countryRepository.GetActiveListAsync(
                cancellationToken);

            var result = countries.Select(x => new CountryDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Slug = x.Slug,
                FlagUrl = x.FlagUrl,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            }).ToList();

            return Result<List<CountryDto>>.Success(result);
        }
    }
}
