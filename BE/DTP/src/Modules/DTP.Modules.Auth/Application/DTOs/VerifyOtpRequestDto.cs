using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.DTOs
{
    public class VerifyOtpRequestDto
    {
        public string Email { get; set; } = default!;
        public string OtpCode { get; set; } = default!;

        public string? IpAddress { get; set; } = default!;

        public string? UserAgent { get; set; } = default!;
    }
}
