using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.ProductVariants
{
    public class GetProductVariantsQuery : IRequest<List<ProductVariantDto>>
    {
        public Guid ProductId { get; set; }

        public GetProductVariantsQuery(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductVariantsQueryHandler
    : IRequestHandler<GetProductVariantsQuery, List<ProductVariantDto>>
    {
        private readonly IProductVariantRepository _repository;

        public GetProductVariantsQueryHandler(IProductVariantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductVariantDto>> Handle(
            GetProductVariantsQuery request,
            CancellationToken cancellationToken)
        {
            var variants = await _repository.GetListAsync(
                request.ProductId,
                cancellationToken);

            return variants.Select(x => new ProductVariantDto
            {
                Id = x.Id,
                Sku = x.Sku,
                Name = x.Name,
                Price = x.Price,
                OriginalPrice = x.OriginalPrice,
                DurationDays = x.DurationDays,
                DataAmount = x.DataAmount,
                DataUnit = x.DataUnit,
                IsUnlimited = x.IsUnlimited,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            }).ToList();
        }
    }
}
