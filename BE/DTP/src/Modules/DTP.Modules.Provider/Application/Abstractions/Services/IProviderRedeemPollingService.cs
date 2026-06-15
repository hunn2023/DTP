using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderRedeemPollingService
    {
        Task PollPendingRedeemsAsync(
            int take,
            CancellationToken cancellationToken = default);

        Task SendDoneRedeemEmailsAsync(
            int take,
            CancellationToken cancellationToken = default);
    }
}
