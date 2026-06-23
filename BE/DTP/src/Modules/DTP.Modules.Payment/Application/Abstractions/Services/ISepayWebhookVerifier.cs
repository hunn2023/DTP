using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface ISepayWebhookVerifier
    {
        bool Verify(IHeaderDictionary headers, string rawBody);
    }
}
