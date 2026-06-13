using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record UpdateContentArticleCommand(
     Guid Id,
     string Title,
     string Slug,
     string? Summary,
     string Content,
     string? ThumbnailUrl,
     string? AuthorName,
     string? CategoryCode,
     string? Tags,
     ContentArticleStatus Status,
     bool IsFeatured,
     int SortOrder) : IRequest<Result<ContentArticleDto>>;


    public class UpdateContentArticleCommandHandler
    : IRequestHandler<UpdateContentArticleCommand, Result<ContentArticleDto>>
    {
        private readonly IContentArticleService _service;

        public UpdateContentArticleCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result<ContentArticleDto>> Handle(
            UpdateContentArticleCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UpdateAsync(
                request.Id,
                request.Title,
                request.Slug,
                request.Summary,
                request.Content,
                request.ThumbnailUrl,
                request.AuthorName,
                request.CategoryCode,
                request.Tags,
                request.Status,
                request.IsFeatured,
                request.SortOrder,
                cancellationToken);
        }
    }
}
