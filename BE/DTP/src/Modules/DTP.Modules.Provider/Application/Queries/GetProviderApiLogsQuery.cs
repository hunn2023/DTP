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
    public class GetProviderApiLogsQuery : IRequest<PagedResultDto<ProviderApiLogDto>>
    {
        public Guid? ProviderId { get; set; }

        public ProviderApiLogType? LogType { get; set; }

        public bool? IsSuccess { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetProviderApiLogsQueryHandler
       : IRequestHandler<GetProviderApiLogsQuery, PagedResultDto<ProviderApiLogDto>>
    {
        private readonly IProviderApiLogRepository _repository;

        public GetProviderApiLogsQueryHandler(IProviderApiLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<ProviderApiLogDto>> Handle(
            GetProviderApiLogsQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetPagedAsync(
                request.ProviderId,
                request.LogType,
                request.IsSuccess,
                request.FromDate,
                request.ToDate,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
