using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Enums.DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Providers
{
    public class UpdateExternalProviderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public ProviderType Type { get; set; }

        public string? ApiBaseUrl { get; set; }

        public bool IsActive { get; set; }

        public string? Description { get; set; }
    }

    public class UpdateExternalProviderCommandHandler
        : IRequestHandler<UpdateExternalProviderCommand, bool>
    {
        private readonly IExternalProviderRepository _repository;
        private readonly IProviderUnitOfWork _unitOfWork;

        public UpdateExternalProviderCommandHandler(
            IExternalProviderRepository repository,
            IProviderUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateExternalProviderCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new Exception("Provider id is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Provider name is required.");

            var provider = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (provider == null)
                throw new Exception("Provider not found.");

            provider.Update(
                request.Name,
                request.Type,
                request.ApiBaseUrl,
                request.IsActive,
                request.Description);

            _repository.Update(provider);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
