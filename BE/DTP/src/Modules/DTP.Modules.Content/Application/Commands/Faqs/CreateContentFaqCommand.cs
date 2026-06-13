using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record CreateContentFaqCommand(
     string Question,
     string Answer,
     string? CategoryCode,
     int SortOrder,
     bool IsActive) : IRequest<Result<ContentFaqDto>>;


    public class CreateContentFaqCommandHandler
    : IRequestHandler<CreateContentFaqCommand, Result<ContentFaqDto>>
    {
        private readonly IContentFaqService _service;

        public CreateContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result<ContentFaqDto>> Handle(
            CreateContentFaqCommand request,
            CancellationToken cancellationToken)
        {
            return _service.CreateAsync(
                request.Question,
                request.Answer,
                request.CategoryCode,
                request.SortOrder,
                request.IsActive,
                cancellationToken);
        }
    }
}
