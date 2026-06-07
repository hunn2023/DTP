using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Queries
{
    public class GetProviderOrdersQuery : IRequest<PagedResultDto<ProviderOrderDto>>
    {
        public Guid? ProviderId { get; set; }

        public ProviderOrderStatus? Status { get; set; }

        public string? Keyword { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetProviderOrdersQueryHandler
        : IRequestHandler<GetProviderOrdersQuery, PagedResultDto<ProviderOrderDto>>
    {
        private readonly IProviderOrderRepository _repository;

        public GetProviderOrdersQueryHandler(IProviderOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<ProviderOrderDto>> Handle(
            GetProviderOrdersQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetPagedAsync(
                request.ProviderId,
                request.Status,
                request.Keyword,
                request.FromDate,
                request.ToDate,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
