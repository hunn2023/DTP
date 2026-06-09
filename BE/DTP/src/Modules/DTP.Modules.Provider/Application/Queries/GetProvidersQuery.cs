using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Queries
{
    public class GetProvidersQuery : IRequest<List<ExternalProviderDto>>
    {
    }

    public class GetProvidersQueryHandler
        : IRequestHandler<GetProvidersQuery, List<ExternalProviderDto>>
    {
        private readonly IExternalProviderRepository _repository;

        public GetProvidersQueryHandler(IExternalProviderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ExternalProviderDto>> Handle(
            GetProvidersQuery request,
            CancellationToken cancellationToken)
        {
            var providers = await _repository.GetActiveAsync(cancellationToken);

            return providers.Select(x => new ExternalProviderDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Type = x.Type,
                ApiBaseUrl = x.ApiBaseUrl,
                IsActive = x.IsActive,
                Description = x.Description
            }).ToList();
        }
    }
}
