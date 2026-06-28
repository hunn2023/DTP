using DTP.Modules.Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public sealed class PaymentProviderValidationResult
    {
        public bool IsValid { get; private init; }

        public string? ErrorMessage { get; private init; }

        public string? Reason { get; private init; }

        public PaymentProvider? Provider { get; private init; }

        public static PaymentProviderValidationResult Success(PaymentProvider provider)
        {
            return new PaymentProviderValidationResult
            {
                IsValid = true,
                Provider = provider
            };
        }

        public static PaymentProviderValidationResult Failure(
            string errorMessage,
            string reason)
        {
            return new PaymentProviderValidationResult
            {
                IsValid = false,
                ErrorMessage = errorMessage,
                Reason = reason
            };
        }
    }
}
