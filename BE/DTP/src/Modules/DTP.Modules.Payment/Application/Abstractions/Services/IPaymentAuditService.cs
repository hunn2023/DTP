using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentAuditService
    {
        Task WriteAsync(
            string action,
            string status,
            Guid? entityId,
            string description,
            object? oldValues,
            object? newValues,
            CancellationToken cancellationToken = default);
    }
}
