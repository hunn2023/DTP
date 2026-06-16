using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public record GetHomeEsimProductsQuery()
     : IRequest<Result<List<HomeEsimCountryProductDto>>>;

    public class GetHomeEsimProductsQueryHandler
        : IRequestHandler<GetHomeEsimProductsQuery, Result<List<HomeEsimCountryProductDto>>>
    {
        private readonly IProductService _productService;

        public GetHomeEsimProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<List<HomeEsimCountryProductDto>>> Handle(
            GetHomeEsimProductsQuery request,
            CancellationToken cancellationToken)
        {
            return await _productService.GetHomeEsimProductsAsync(cancellationToken);
        }
    }
}
