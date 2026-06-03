using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.Countries
{

    public class CreateCountryCommand : IRequest<Guid>
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? FlagUrl { get; set; }
        public int SortOrder { get; set; }
    }

    public class CreateCountryCommandHandler
       : IRequestHandler<CreateCountryCommand, Guid>
    {
        private readonly ICountryService _countryService;

        public CreateCountryCommandHandler(
            ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Guid> Handle(
            CreateCountryCommand request,
            CancellationToken cancellationToken)
        {
            return await _countryService.CreateAsync(
                request.Code,
                request.Name,
                request.Slug,
                request.FlagUrl,
                request.SortOrder,
                cancellationToken);
        }
    }
}
