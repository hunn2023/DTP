using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Caching;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.Providers
{
    public class UpdateProviderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ApiBaseUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? WebhookUrl { get; set; }
        public string? SupportEmail { get; set; }
        public bool IsActive { get; set; }
    }


    public class UpdateProviderCommandHandler : IRequestHandler<UpdateProviderCommand, bool>
    {
        private readonly IProviderRepository _providerRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProviderService _providerService;
        private readonly ICacheService _cacheService;
        public UpdateProviderCommandHandler(
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

        public async Task<bool> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = await _providerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (provider == null)
                throw new Exception("Provider not found.");

            if (await _providerRepository.ExistsByCodeAsync(request.Code, request.Id, cancellationToken))
                throw new Exception("Provider code already exists.");

            if (await _providerRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
                throw new Exception("Provider name already exists.");

            provider.Update(
                request.Code,
                request.Name,
                request.ApiBaseUrl,
                request.ApiKey,
                request.ApiSecret,
                request.WebhookUrl,
                request.SupportEmail,
                request.IsActive);

            _providerRepository.Update(provider);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _providerService.ClearProviderCacheAsync(cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:providers:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:phonecards:", cancellationToken);
            return true;
        }
    }
}
