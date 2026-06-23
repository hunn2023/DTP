using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public sealed class SepayQrBuilder : ISepayQrBuilder
    {
        private readonly SepayOptions _options;

        public SepayQrBuilder(IOptions<SepayOptions> options)
        {
            _options = options.Value;
        }

        public string BuildQrUrl(
            string accountNumber,
            string bankCode,
            decimal amount,
            string description)
        {
            var amountInt = decimal.ToInt64(decimal.Round(amount, 0));

            var query = new Dictionary<string, string?>
            {
                ["acc"] = accountNumber,
                ["bank"] = bankCode,
                ["amount"] = amountInt.ToString(),
                ["des"] = description,
                ["template"] = "compact"
            };

            return QueryHelpers.AddQueryString(_options.QrBaseUrl, query);
        }
    }
}
