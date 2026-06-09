using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.Abstractions.Services
{
    public interface IRateLimitService
    {
        Task<bool> IsAllowedAsync(
            string key,
            int maxRequest,
            TimeSpan window);
    }
}
