using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.Countries
{
    public class DeleteCountryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteCountryCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteCountryCommandHandler
    : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly ICountryService _countryService;

        public DeleteCountryCommandHandler(
            ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<bool> Handle(
            DeleteCountryCommand request,
            CancellationToken cancellationToken)
        {
            await _countryService.DeleteAsync(
                request.Id,
                cancellationToken);

            return true;
        }
    }
}
