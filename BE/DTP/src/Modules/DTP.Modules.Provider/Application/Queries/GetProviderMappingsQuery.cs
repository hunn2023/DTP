using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Queries
{
    public class GetProviderMappingsQuery : IRequest<PagedResultDto<ProviderProductMappingDto>>
    {
        public Guid? ProviderId { get; set; }

        public ProviderProductType? ProductType { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public bool? IsActive { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetProviderMappingsQueryHandler
        : IRequestHandler<GetProviderMappingsQuery, PagedResultDto<ProviderProductMappingDto>>
    {
        private readonly IProviderProductMappingRepository _repository;

        public GetProviderMappingsQueryHandler(IProviderProductMappingRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<ProviderProductMappingDto>> Handle(
            GetProviderMappingsQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetPagedAsync(
                request.ProviderId,
                request.ProductType,
                request.ProductId,
                request.ProductVariantId,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
