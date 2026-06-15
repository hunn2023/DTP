using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Enums;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductContentService
    {
        Task<Result<ProductContentDto>> CreateAsync(
            CreateProductContentDto request,
            CancellationToken cancellationToken = default);

        Task<Result<ProductContentDto>> UpdateAsync(
            Guid id,
            UpdateProductContentDto request,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<ProductContentDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<List<ProductContentDto>>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        Task<Result<List<ProductContentDto>>> GetByProductIdAndTypeAsync(
            Guid productId,
            ProductContentType contentType,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);
    }
}
