using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Providers
{
    public class GetProviderByIdQuery : IRequest<Result<ProviderDto?>>
    {
        public Guid Id { get; set; }

        public GetProviderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
    public class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery,
        Result<ProviderDto?>>
    {
        private readonly IProviderRepository _providerRepository;

        public GetProviderByIdQueryHandler(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<Result<ProviderDto?>> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
        {
            var provider = await _providerRepository.GetByIdAsync(request.Id, cancellationToken);

            var result = new ProviderDto
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

            return Result<ProviderDto?>.Success(result); 
        }
    }

}
