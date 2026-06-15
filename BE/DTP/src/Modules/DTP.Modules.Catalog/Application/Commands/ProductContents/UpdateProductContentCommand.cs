using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Enums;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.ProductContents
{
    public class UpdateProductContentCommand : IRequest<Result<ProductContentDto>>
    {
        public Guid Id { get; set; }

        public ProductContentType ContentType { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string BodyHtml { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateProductContentCommandHandler
        : IRequestHandler<UpdateProductContentCommand, Result<ProductContentDto>>
    {
        private readonly IProductContentService _productContentService;

        public UpdateProductContentCommandHandler(
            IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }

        public async Task<Result<ProductContentDto>> Handle(
            UpdateProductContentCommand request,
            CancellationToken cancellationToken)
        {
            var dto = new UpdateProductContentDto
            {
                ContentType = request.ContentType,
                Title = request.Title,
                Summary = request.Summary,
                BodyHtml = request.BodyHtml,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            return await _productContentService.UpdateAsync(
                request.Id,
                dto,
                cancellationToken);
        }
    }
}
