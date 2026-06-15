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

namespace DTP.Modules.Catalog.Application.Queries.ProductContents
{
    public class GetProductContentsByProductIdAndTypeQuery : IRequest<Result<List<ProductContentDto>>>
    {
        public Guid ProductId { get; set; }

        public ProductContentType ContentType { get; set; }

        public bool OnlyActive { get; set; }

        public GetProductContentsByProductIdAndTypeQuery(
            Guid productId,
            ProductContentType contentType,
            bool onlyActive = false)
        {
            ProductId = productId;
            ContentType = contentType;
            OnlyActive = onlyActive;
        }
    }

    public class GetProductContentsByProductIdAndTypeQueryHandler
        : IRequestHandler<GetProductContentsByProductIdAndTypeQuery, Result<List<ProductContentDto>>>
    {
        private readonly IProductContentService _productContentService;

        public GetProductContentsByProductIdAndTypeQueryHandler(
            IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }

        public async Task<Result<List<ProductContentDto>>> Handle(
            GetProductContentsByProductIdAndTypeQuery request,
            CancellationToken cancellationToken)
        {
            return await _productContentService.GetByProductIdAndTypeAsync(
                request.ProductId,
                request.ContentType,
                request.OnlyActive,
                cancellationToken);
        }
    }
}
