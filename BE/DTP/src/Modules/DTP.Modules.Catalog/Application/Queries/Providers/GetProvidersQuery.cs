using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Providers
{
    public class GetProvidersQuery : IRequest<List<ProviderDto>>
    {
        public string? Keyword { get; set; }
    }
    public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, List<ProviderDto>>
    {
        private readonly IProviderRepository _providerRepository;

        public GetProvidersQueryHandler(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<List<ProviderDto>> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
        {
            var providers = await _providerRepository.GetListAsync(request.Keyword, cancellationToken);

            return providers.Select(x => new ProviderDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ApiBaseUrl = x.ApiBaseUrl,
                ApiKey = x.ApiKey,
                ApiSecret = x.ApiSecret,
                WebhookUrl = x.WebhookUrl,
                SupportEmail = x.SupportEmail,
                IsActive = x.IsActive
            }).ToList();
        }
    }

}
