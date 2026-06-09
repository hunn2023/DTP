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
    public class DeleteProviderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteProviderCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProviderCommandHandler : IRequestHandler<DeleteProviderCommand, bool>
    {
        private readonly IProviderRepository _providerRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProviderService _providerService;
        private readonly ICacheService _cacheService;
        public DeleteProviderCommandHandler(
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

        public async Task<bool> Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = await _providerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (provider == null)
                throw new Exception("Provider not found.");

            provider.Deactivate();

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
