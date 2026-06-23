using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface ISepayQrBuilder
    {
        string BuildQrUrl(
            string accountNumber,
            string bankCode,
            decimal amount,
            string description);
    }
}
