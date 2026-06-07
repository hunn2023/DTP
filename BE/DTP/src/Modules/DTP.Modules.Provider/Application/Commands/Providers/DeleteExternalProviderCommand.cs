using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Providers
{
    public class DeleteExternalProviderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteExternalProviderCommandHandler
        : IRequestHandler<DeleteExternalProviderCommand, bool>
    {
        private readonly IExternalProviderRepository _repository;
        private readonly IProviderUnitOfWork _unitOfWork;

        public DeleteExternalProviderCommandHandler(
            IExternalProviderRepository repository,
            IProviderUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteExternalProviderCommand request,
            CancellationToken cancellationToken)
        {
            var provider = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (provider == null)
                throw new Exception("Provider not found.");

            _repository.Remove(provider);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
