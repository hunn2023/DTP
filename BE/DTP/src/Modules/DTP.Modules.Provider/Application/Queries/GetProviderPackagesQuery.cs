using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Queries
{
    public class GetProviderPackagesQuery : IRequest<GetProviderPackagesResult>
    {
        public Guid? ProviderId { get; set; }
        public string? Keyword { get; set; }
        public string? SyncStatus { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetProviderPackagesResult
    {
        public IReadOnlyList<ProviderPackageDto> Items { get; set; } = Array.Empty<ProviderPackageDto>();
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class GetProviderPackagesQueryHandler
        : IRequestHandler<GetProviderPackagesQuery, GetProviderPackagesResult>
    {
        private readonly IProviderPackageProductRepository _repository;

        public GetProviderPackagesQueryHandler(
            IProviderPackageProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetProviderPackagesResult> Handle(
            GetProviderPackagesQuery request,
            CancellationToken cancellationToken)
        {
            var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

            var result = await _repository.GetPagedAsync(
                request.ProviderId,
                request.Keyword,
                request.SyncStatus,
                pageIndex,
                pageSize,
                cancellationToken);

            return new GetProviderPackagesResult
            {
                Items = result.Items.Select(x => new ProviderPackageDto
                {
                    Id = x.Id,
                    ProviderId = x.ProviderId,
                    ProviderSku = x.ProviderSku,
                    ProviderProductId = x.ProviderProductId,
                    Name = x.Name,
                    Model = x.Model,
                    Regional = x.Regional,
                    RegionId = x.RegionId,
                    Price = x.Price,
                    CurrencyCode = x.CurrencyCode,
                    SyncStatus = x.SyncStatus,
                    ErrorMessage = x.ErrorMessage,
                    LastSyncedAt = x.LastSyncedAt
                }).ToList(),
                Total = result.Total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
