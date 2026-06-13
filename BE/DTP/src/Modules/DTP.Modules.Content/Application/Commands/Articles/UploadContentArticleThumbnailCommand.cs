using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public class UploadContentArticleThumbnailCommand : IRequest<Result<ContentArticleDto>>
    {
        public Guid ArticleId { get; set; }

        public IFormFile File { get; set; } = default!;
    }


    public class UploadContentArticleThumbnailCommandHandler
       : IRequestHandler<UploadContentArticleThumbnailCommand, Result<ContentArticleDto>>
    {
        private readonly IContentArticleService _contentArticleService;

        public UploadContentArticleThumbnailCommandHandler(
            IContentArticleService contentArticleService)
        {
            _contentArticleService = contentArticleService;
        }

        public async Task<Result<ContentArticleDto>> Handle(
            UploadContentArticleThumbnailCommand request,
            CancellationToken cancellationToken)
        {
            return await _contentArticleService.UploadThumbnailAsync(
                request.ArticleId,
                request.File,
                cancellationToken);
        }
    }
}
