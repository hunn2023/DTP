using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class ErrorDto
    {
        public string? Field { get; set; }

        public string Message { get; set; } = default!;

        public string? Code { get; set; }

        public ErrorDto()
        {
        }

        public ErrorDto(string message)
        {
            Message = message;
        }

        public ErrorDto(string? field, string message, string? code = null)
        {
            Field = field;
            Message = message;
            Code = code;
        }
    }
}
