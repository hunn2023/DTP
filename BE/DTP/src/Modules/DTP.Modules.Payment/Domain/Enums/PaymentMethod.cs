using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Enums
{
    public enum PaymentMethod
    {
        BankTransferQr = 1,
        EWallet = 2,
        Card = 3
    }
}
