using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductFaqRepository : IRepositoryBase<ProductFaq>
    {
        Task<List<ProductFaq>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);
    }
}
