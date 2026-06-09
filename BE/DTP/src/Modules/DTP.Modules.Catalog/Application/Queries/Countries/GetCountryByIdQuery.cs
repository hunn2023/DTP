using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Countries
{
    public class GetCountryByIdQuery : IRequest<Result<CountryDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetCountryByIdQueryhandler
     : IRequestHandler<GetCountryByIdQuery, Result<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;

        public GetCountryByIdQueryhandler(
            ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<Result<CountryDto>> Handle(
            GetCountryByIdQuery request,
            CancellationToken cancellationToken)
        {
            var countries = await _countryRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if(countries == null)
            {
                return new Result<CountryDto>();
            }

            var result = new CountryDto
            {
                Id = countries.Id,
                Code = countries.Code,
                Name = countries.Name,
                Slug = countries.Slug,
                FlagUrl = countries.FlagUrl,
                SortOrder = countries.SortOrder,
                IsActive = countries.IsActive
            };

            return Result<CountryDto>.Success(result);
        }
    }
}
