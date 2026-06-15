using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderFulfillmentService
    {
        Task ConfirmAndRedeemAsync(
        Guid dtpOrderId,
        CancellationToken cancellationToken = default);
    }
}
