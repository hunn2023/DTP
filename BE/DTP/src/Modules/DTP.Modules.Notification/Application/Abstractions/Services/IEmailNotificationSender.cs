using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Abstractions.Services
{
    public interface IEmailNotificationSender
    {
        Task SendAsync(
            string toEmail,
            string subject,
            string body,
            CancellationToken cancellationToken = default);
    }
}
