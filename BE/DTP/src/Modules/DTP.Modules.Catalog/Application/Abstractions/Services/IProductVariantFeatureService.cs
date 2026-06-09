using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductVariantFeatureService
    {
        Task<Result<List<ProductVariantFeatureDto>>> GetByProductVariantIdAsync(
            Guid productVariantId,
            CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(
            Guid productVariantId,
            string text,
            string? icon,
            int? sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> UpdateAsync(
            Guid id,
            string text,
            string? icon,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
