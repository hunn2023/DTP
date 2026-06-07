using DTP.Modules.Ordering.Application.DTOs;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Abstractions.Services
{
    public interface IOrderingCatalogService
    {

        Task<Result<OrderingProductSnapshotDto?>> GetProductForCheckoutAsync(
            Guid productId,
            Guid? productVariantId,
            CancellationToken cancellationToken = default);
    }
}
