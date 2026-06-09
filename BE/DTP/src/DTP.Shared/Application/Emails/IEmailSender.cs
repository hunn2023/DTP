using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Emails
{
    public interface IEmailSender
    {
        Task SendAsync(
            string to,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default);

        Task SendAsync(
            EmailMessage message,
            CancellationToken cancellationToken = default);
    }
}
