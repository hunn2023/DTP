using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.Abstractions.Services
{

    public interface IJwtService
    {
        string GenerateAccessToken(
            Guid userId,
            string email,
            string fullName,
            List<string> roles,
            List<string> permissions);

        string GenerateRefreshToken();

        string HashToken(string token);
    }
}
