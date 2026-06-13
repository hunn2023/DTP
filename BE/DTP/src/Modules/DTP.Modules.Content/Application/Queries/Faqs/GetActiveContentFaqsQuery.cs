using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Faqs
{
    public record GetActiveContentFaqsQuery(
     string? CategoryCode) : IRequest<Result<IReadOnlyList<ContentFaqDto>>>;


    public class GetActiveContentFaqsQueryHandler
    : IRequestHandler<GetActiveContentFaqsQuery, Result<IReadOnlyList<ContentFaqDto>>>
    {
        private readonly IContentFaqService _service;

        public GetActiveContentFaqsQueryHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result<IReadOnlyList<ContentFaqDto>>> Handle(
            GetActiveContentFaqsQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetActiveAsync(
                request.CategoryCode,
                cancellationToken);
        }
    }
}
