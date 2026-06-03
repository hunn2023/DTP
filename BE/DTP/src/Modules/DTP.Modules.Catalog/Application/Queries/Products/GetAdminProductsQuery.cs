using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public class GetAdminProductsQuery
         : IRequest<PagedResultDto<ProductDto>>
    {
        public string? Keyword { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CountryId { get; set; }

        public Guid? CarrierId { get; set; }

        public bool? IsActive { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetAdminProductsQueryHandler
      : IRequestHandler<GetAdminProductsQuery, PagedResultDto<ProductDto>>
    {
        private readonly IProductService _productService;

        public GetAdminProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<PagedResultDto<ProductDto>> Handle(
            GetAdminProductsQuery request,
            CancellationToken cancellationToken)
        {
            return await _productService.GetPagedAsync(
                request.Keyword,
                request.CategoryId,
                request.CountryId,
                request.CarrierId,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
