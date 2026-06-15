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
    public class GetProductContentByIdQuery : IRequest<Result<ProductContentDto?>>
    {
        public Guid Id { get; set; }

        public GetProductContentByIdQuery(Guid id)
        {
            Id = id;
        }
    }


    public class GetProductContentByIdQueryHandler
        : IRequestHandler<GetProductContentByIdQuery, Result<ProductContentDto?>?>
    {
        private readonly IProductContentService _productContentService;

        public GetProductContentByIdQueryHandler(
            IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }

        public async Task<Result<ProductContentDto?>?> Handle(
            GetProductContentByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _productContentService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
