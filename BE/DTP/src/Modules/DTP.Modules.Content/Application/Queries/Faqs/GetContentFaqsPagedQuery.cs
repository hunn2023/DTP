using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Faqs
{
    public record GetContentFaqsPagedQuery(
        string? Keyword,
        string? CategoryCode,
        bool? IsActive,
        int PageIndex,
        int PageSize) : IRequest<Result<PagedResultDto<ContentFaqDto>>>;


    public class GetContentFaqsPagedQueryHandler
    : IRequestHandler<GetContentFaqsPagedQuery, Result<PagedResultDto<ContentFaqDto>>>
    {
        private readonly IContentFaqService _service;

        public GetContentFaqsPagedQueryHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result<PagedResultDto<ContentFaqDto>>> Handle(
            GetContentFaqsPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetPagedAsync(
                request.Keyword,
                request.CategoryCode,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
