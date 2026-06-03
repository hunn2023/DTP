using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class BooleanResultDto
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public BooleanResultDto()
        {
        }

        public BooleanResultDto(bool success, string? message = null)
        {
            Success = success;
            Message = message;
        }
    }
}
