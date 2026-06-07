using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Mappings
{
    public class DeleteProviderProductMappingCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProviderProductMappingCommandHandler
        : IRequestHandler<DeleteProviderProductMappingCommand, bool>
    {
        private readonly IProviderProductMappingRepository _repository;
        private readonly IProviderUnitOfWork _unitOfWork;

        public DeleteProviderProductMappingCommandHandler(
            IProviderProductMappingRepository repository,
            IProviderUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteProviderProductMappingCommand request,
            CancellationToken cancellationToken)
        {
            var mapping = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (mapping == null)
                throw new Exception("Provider product mapping not found.");

            _repository.Remove(mapping);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
