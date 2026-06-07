using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record UpdateContentFaqCommand(
     Guid Id,
     string Question,
     string Answer,
     string? CategoryCode,
     int SortOrder,
     bool IsActive) : IRequest<ContentFaqDto>;


    public class UpdateContentFaqCommandHandler
    : IRequestHandler<UpdateContentFaqCommand, ContentFaqDto>
    {
        private readonly IContentFaqService _service;

        public UpdateContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<ContentFaqDto> Handle(
            UpdateContentFaqCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UpdateAsync(
                request.Id,
                request.Question,
                request.Answer,
                request.CategoryCode,
                request.SortOrder,
                request.IsActive,
                cancellationToken);
        }
    }
}
