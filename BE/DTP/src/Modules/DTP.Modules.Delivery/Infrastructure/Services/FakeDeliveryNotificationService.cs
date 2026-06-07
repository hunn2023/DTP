using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class FakeDeliveryNotificationService : IDeliveryNotificationService
    {
        public Task SendEsimDeliveryEmailAsync(
            string recipientEmail,
            string? customerName,
            string orderCode,
            List<EsimProfileDto> esimProfiles,
            CancellationToken cancellationToken = default)
        {
            // TODO: Sau này thay bằng Notification Module thật.
            return Task.CompletedTask;

        }
    }
}
