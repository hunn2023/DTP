using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductPriceService
    {
        Task<Result<Guid>> CreateAsync(
             Guid productId,
             Guid? productVariantId,
             string currency,
             decimal originalPrice,
             decimal salePrice,
             decimal costPrice,
             DateTime? startDate,
             DateTime? endDate,
             CancellationToken cancellationToken = default);

        Task<Result> DeleteProductPriceAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string currency,
            decimal originalPrice,
            decimal salePrice,
            decimal costPrice,
            DateTime? startDate,
            DateTime? endDate,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
