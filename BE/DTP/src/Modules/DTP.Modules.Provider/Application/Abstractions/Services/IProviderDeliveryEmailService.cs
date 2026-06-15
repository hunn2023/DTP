using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderDeliveryEmailService
    {
        Task SendEsimEmailAsync(
            ProviderRedeem redeem,
            CancellationToken cancellationToken = default);

        Task SendInsuranceEmailAsync(
            ProviderRedeem redeem,
            CancellationToken cancellationToken = default);
    }
}
