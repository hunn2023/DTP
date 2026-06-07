using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.Countries
{
    public class UpdateCountryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? FlagUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCountryCommandHandler
       : IRequestHandler<UpdateCountryCommand, Result>
    {
        private readonly ICountryService _countryService;

        public UpdateCountryCommandHandler(
            ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Result> Handle(
            UpdateCountryCommand request,
            CancellationToken cancellationToken)
        {
            return await _countryService.UpdateAsync(
                  request.Id,
                  request.Code,
                  request.Name,
                  request.Slug,
                  request.FlagUrl,
                  request.SortOrder,
                  request.IsActive,
                  cancellationToken);
        }
    }
}
