
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Application.Commands.Countries
{
    public class UploadCountryFlagCommand : IRequest<Result<CountryDto>>
    {
        public Guid CountryId { get; set; }

        public IFormFile File { get; set; } = default!;

    }


    public class UploadCountryFlagCommandHandler
    : IRequestHandler<UploadCountryFlagCommand, Result<CountryDto>>
    {
        private readonly ICountryService _countryService;

        public UploadCountryFlagCommandHandler(
            ICountryService countryService
            )
        {
            _countryService = countryService;
        }

        public async Task<Result<CountryDto>> Handle(
            UploadCountryFlagCommand request,
            CancellationToken cancellationToken)
        {
           return await _countryService.UploadFlagAsync(
                request.CountryId,
                request.File,
                cancellationToken);
        }
    }
}
