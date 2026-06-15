using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.ProductContents
{
    public class GetProductContentsByProductIdQuery : IRequest<Result<List<ProductContentDto>>>
    {
        public Guid ProductId { get; set; }

        public bool OnlyActive { get; set; }

        public GetProductContentsByProductIdQuery(
            Guid productId,
            bool onlyActive = false)
        {
            ProductId = productId;
            OnlyActive = onlyActive;
        }
    }

    public class GetProductContentsByProductIdQueryHandler
       : IRequestHandler<GetProductContentsByProductIdQuery, Result<List<ProductContentDto>>>
    {
        private readonly IProductContentService _productContentService;

        public GetProductContentsByProductIdQueryHandler(
            IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }

        public async Task<Result<List<ProductContentDto>>> Handle(
            GetProductContentsByProductIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _productContentService.GetByProductIdAsync(
                request.ProductId,
                request.OnlyActive,
                cancellationToken);
        }
    }
}
