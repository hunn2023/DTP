using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderApiLogRepository
    {
        Task AddAsync(
            ProviderApiLog log,
            CancellationToken cancellationToken = default);
    }
}
