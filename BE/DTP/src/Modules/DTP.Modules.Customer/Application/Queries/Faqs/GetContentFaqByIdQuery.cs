using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Faqs
{
    public record GetContentFaqByIdQuery(Guid Id) : IRequest<Result<ContentFaqDto?>>;


    public class GetContentFaqByIdQueryHandler
    : IRequestHandler<GetContentFaqByIdQuery, Result<ContentFaqDto?>>
    {
        private readonly IContentFaqService _service;

        public GetContentFaqByIdQueryHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result<ContentFaqDto?>> Handle(
            GetContentFaqByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
