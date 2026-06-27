using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Constants
{
    public static class PaymentProviderCodes
    {
        public const string Sepay = "SEPAY";
        public const string VnptEpay = "VNPT_EPAY";
        public const string Momo = "MOMO";
        public const string Vnpay = "VNPAY";
    }

    public static class PaymentMethods
    {
        public const string BankQr = "BankQr";
        public const string EWallet = "EWallet";
        public const string Card = "Card";
        public const string BankTransfer = "BankTransfer";
    }
}
