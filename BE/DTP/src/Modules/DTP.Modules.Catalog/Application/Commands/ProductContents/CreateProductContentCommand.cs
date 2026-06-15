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
    public class CreateProductContentCommand : IRequest<Result<ProductContentDto>>
    {
        public Guid ProductId { get; set; }

        public ProductContentType ContentType { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string BodyHtml { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }


    public class CreateProductContentCommandHandler
        : IRequestHandler<CreateProductContentCommand, Result<ProductContentDto>>
    {
        private readonly IProductContentService _productContentService;

        public CreateProductContentCommandHandler(
            IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }

        public async Task<Result<ProductContentDto>> Handle(
            CreateProductContentCommand request,
            CancellationToken cancellationToken)
        {
            var dto = new CreateProductContentDto
            {
                ProductId = request.ProductId,
                ContentType = request.ContentType,
                Title = request.Title,
                Summary = request.Summary,
                BodyHtml = request.BodyHtml,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            return await _productContentService.CreateAsync(
                dto,
                cancellationToken);
        }
    }
}
