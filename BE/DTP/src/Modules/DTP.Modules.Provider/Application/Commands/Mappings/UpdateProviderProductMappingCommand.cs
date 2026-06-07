using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Mappings
{
    public class UpdateProviderProductMappingCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string ProviderProductCode { get; set; } = default!;

        public string? ProviderProductName { get; set; }

        public decimal? ProviderCostPrice { get; set; }

        public string? CurrencyCode { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateProviderProductMappingCommandHandler
        : IRequestHandler<UpdateProviderProductMappingCommand, bool>
    {
        private readonly IProviderProductMappingRepository _repository;
        private readonly IProviderUnitOfWork _unitOfWork;

        public UpdateProviderProductMappingCommandHandler(
            IProviderProductMappingRepository repository,
            IProviderUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateProviderProductMappingCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new Exception("Mapping id is required.");

            if (string.IsNullOrWhiteSpace(request.ProviderProductCode))
                throw new Exception("Provider product code is required.");

            var mapping = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (mapping == null)
                throw new Exception("Provider product mapping not found.");

            mapping.Update(
                request.ProviderProductCode,
                request.ProviderProductName,
                request.ProviderCostPrice,
                request.CurrencyCode,
                request.IsActive);

            _repository.Update(mapping);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
