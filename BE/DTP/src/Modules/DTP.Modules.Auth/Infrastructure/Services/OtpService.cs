using DTP.Modules.Auth.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        public string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();
        }

        public string HashOtp(string otp)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public bool VerifyOtp(string otp, string otpHash)
        {
            var hash = HashOtp(otp);
            return hash == otpHash;
        }
    }
}
