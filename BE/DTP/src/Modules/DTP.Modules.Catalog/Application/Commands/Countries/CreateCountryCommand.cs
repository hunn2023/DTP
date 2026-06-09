using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.Countries
{

    public class CreateCountryCommand : IRequest<Result<Guid>>
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public string? FlagUrl { get; set; }

        public string? Region { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreateCountryCommandHandler
       : IRequestHandler<CreateCountryCommand, Result<Guid>>
    {
        private readonly ICountryService _countryService;

        public CreateCountryCommandHandler(
            ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Result<Guid>> Handle(
            CreateCountryCommand request,
            CancellationToken cancellationToken)
        {
            return await _countryService.CreateAsync(
                request.Code,
                request.Name,
                request.Slug,
                request.FlagUrl,
                request.Region,
                request.Description,
                request.SortOrder,
                request.IsActive,
                cancellationToken);
        }
    }
}
