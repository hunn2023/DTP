using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.ProductFaqs
{
    public class GetProductFaqsByProductIdQuery : IRequest<Result<List<ProductFaqDto>>>
    {
        public Guid ProductId { get; set; }

        public bool OnlyActive { get; set; }

        public GetProductFaqsByProductIdQuery(
            Guid productId,
            bool onlyActive = false)
        {
            ProductId = productId;
            OnlyActive = onlyActive;
        }
    }

    public class GetProductFaqsByProductIdQueryHandler
        : IRequestHandler<GetProductFaqsByProductIdQuery, Result<List<ProductFaqDto>>>
    {
        private readonly IProductFaqService _productFaqService;

        public GetProductFaqsByProductIdQueryHandler(
            IProductFaqService productFaqService)
        {
            _productFaqService = productFaqService;
        }

        public async Task<Result<List<ProductFaqDto>>> Handle(
            GetProductFaqsByProductIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _productFaqService.GetByProductIdAsync(
                request.ProductId,
                request.OnlyActive,
                cancellationToken);
        }
    }
}
