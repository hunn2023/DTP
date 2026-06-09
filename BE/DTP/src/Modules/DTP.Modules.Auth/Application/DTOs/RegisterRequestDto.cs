using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }

        public string FullName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
