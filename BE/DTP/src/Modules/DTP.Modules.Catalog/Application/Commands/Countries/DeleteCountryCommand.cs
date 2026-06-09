using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Countries
{
    public class DeleteCountryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteCountryCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteCountryCommandHandler
    : IRequestHandler<DeleteCountryCommand, Result>
    {
        private readonly ICountryService _countryService;

        public DeleteCountryCommandHandler(
            ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Result> Handle(
            DeleteCountryCommand request,
            CancellationToken cancellationToken)
        {
            return await _countryService.DeleteAsync(
                  request.Id,
                  cancellationToken);

        }
    }
}
