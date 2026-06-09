using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Caching;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Providers
{
    public class CreateProviderCommand : IRequest<Guid>
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ApiBaseUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? WebhookUrl { get; set; }
        public string? SupportEmail { get; set; }
    }

    public class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, Guid>
    {
        private readonly IProviderRepository _providerRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProviderService _providerService;
        private readonly ICacheService _cacheService;
        public CreateProviderCommandHandler(
            IProviderRepository providerRepository,
            ICatalogUnitOfWork unitOfWork,
            IProviderService providerService,
            ICacheService cacheService)
        {
            _providerRepository = providerRepository;
            _unitOfWork = unitOfWork;
            _providerService = providerService;
            _cacheService = cacheService;
        }

        public async Task<Guid> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Provider code is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Provider name is required.");

            if (await _providerRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
                throw new Exception("Provider code already exists.");

            if (await _providerRepository.ExistsByNameAsync(request.Name, null, cancellationToken))
                throw new Exception("Provider name already exists.");

            var provider = new Provider(
                request.Code,
                request.Name,
                request.ApiBaseUrl,
                request.ApiKey,
                request.ApiSecret,
                request.WebhookUrl,
                request.SupportEmail);

            await _providerRepository.AddAsync(provider, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _providerService.ClearProviderCacheAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync("catalog:providers:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:phonecards:", cancellationToken);

            return provider.Id;
        }
    }
}
