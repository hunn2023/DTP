using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.Providers
{
    public class GetProviderByIdQuery : IRequest<ProviderDto?>
    {
        public Guid Id { get; set; }

        public GetProviderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
    public class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery, ProviderDto?>
    {
        private readonly IProviderRepository _providerRepository;

        public GetProviderByIdQueryHandler(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<ProviderDto?> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
        {
            var provider = await _providerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (provider == null)
                return null;

            return new ProviderDto
            {
                Id = provider.Id,
                Code = provider.Code,
                Name = provider.Name,
                ApiBaseUrl = provider.ApiBaseUrl,
                ApiKey = provider.ApiKey,
                ApiSecret = provider.ApiSecret,
                WebhookUrl = provider.WebhookUrl,
                SupportEmail = provider.SupportEmail,
                IsActive = provider.IsActive
            };
        }
    }

}
