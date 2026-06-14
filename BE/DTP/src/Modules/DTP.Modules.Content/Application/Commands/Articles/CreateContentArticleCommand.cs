using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record CreateContentArticleCommand(
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


    public class CreateContentArticleCommandHandler
    : IRequestHandler<CreateContentArticleCommand, Result<ContentArticleDto>>
    {
        private readonly IContentArticleService _service;

        public CreateContentArticleCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result<ContentArticleDto>> Handle(
            CreateContentArticleCommand request,
            CancellationToken cancellationToken)
        {
            return _service.CreateAsync(
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
