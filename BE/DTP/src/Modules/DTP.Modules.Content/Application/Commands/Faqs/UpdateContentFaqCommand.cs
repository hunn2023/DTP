using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record UpdateContentFaqCommand(
     Guid Id,
     string Question,
     string Answer,
     string? CategoryCode,
     int SortOrder,
     bool IsActive) : IRequest<Result<ContentFaqDto>>;


    public class UpdateContentFaqCommandHandler
    : IRequestHandler<UpdateContentFaqCommand, Result<ContentFaqDto>>
    {
        private readonly IContentFaqService _service;

        public UpdateContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result<ContentFaqDto>> Handle(
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
