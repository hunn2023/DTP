using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums.DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Providers
{
    public class CreateExternalProviderCommand : IRequest<Guid>
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public ProviderType Type { get; set; }

        public string? ApiBaseUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Description { get; set; }
    }

    public class CreateExternalProviderCommandHandler
        : IRequestHandler<CreateExternalProviderCommand, Guid>
    {
        private readonly IExternalProviderRepository _repository;
        private readonly IProviderUnitOfWork _unitOfWork;

        public CreateExternalProviderCommandHandler(
            IExternalProviderRepository repository,
            IProviderUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            CreateExternalProviderCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Provider code is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Provider name is required.");

            var code = request.Code.Trim().ToUpperInvariant();

            var exists = await _repository.ExistsCodeAsync(
                code,
                null,
                cancellationToken);

            if (exists)
                throw new Exception("Provider code already exists.");

            var provider = new ExternalProvider(
                code,
                request.Name,
                request.Type,
                request.ApiBaseUrl,
                request.IsActive);

            provider.Update(
                request.Name,
                request.Type,
                request.ApiBaseUrl,
                request.IsActive,
                request.Description);

            await _repository.AddAsync(provider, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return provider.Id;
        }
    }
}
