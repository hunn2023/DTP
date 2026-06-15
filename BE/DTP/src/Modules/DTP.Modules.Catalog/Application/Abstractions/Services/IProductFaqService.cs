using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductFaqService
    {
        Task<Result<ProductFaqDto>> CreateAsync(
            CreateProductFaqDto request,
            CancellationToken cancellationToken = default);

        Task<Result<ProductFaqDto>> UpdateAsync(
            Guid id,
            UpdateProductFaqDto request,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<ProductFaqDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<List<ProductFaqDto>>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);
    }
}
