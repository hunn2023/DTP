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
    public class GetProductFaqByIdQuery : IRequest<Result<ProductFaqDto?>>
    {
        public Guid Id { get; set; }

        public GetProductFaqByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetProductFaqByIdQueryHandler
     : IRequestHandler<GetProductFaqByIdQuery, Result<ProductFaqDto?>>
    {
        private readonly IProductFaqService _productFaqService;

        public GetProductFaqByIdQueryHandler(
            IProductFaqService productFaqService)
        {
            _productFaqService = productFaqService;
        }

        public async Task<Result<ProductFaqDto?>> Handle(
            GetProductFaqByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _productFaqService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
