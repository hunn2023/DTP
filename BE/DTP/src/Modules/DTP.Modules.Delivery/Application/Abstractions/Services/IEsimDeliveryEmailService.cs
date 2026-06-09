using DTP.Modules.Delivery.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Services
{
    public interface IEsimDeliveryEmailService
    {
        Task SendEsimQrEmailAsync(
            DeliveryDto delivery,
            CancellationToken cancellationToken = default);
    }
}
