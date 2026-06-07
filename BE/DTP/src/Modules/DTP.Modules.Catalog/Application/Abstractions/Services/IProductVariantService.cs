using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductVariantService
    {
        Task<Result<Guid>> CreateAsync(
            Guid productId,
            string? sku,
            string name,
            decimal price,
            decimal? originalPrice,
            int? durationDays,
            decimal? dataAmount,
            string? dataUnit,
            bool isUnlimited,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string? sku,
            string name,
            decimal price,
            decimal? originalPrice,
            int? durationDays,
            decimal? dataAmount,
            string? dataUnit,
            bool isUnlimited,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
