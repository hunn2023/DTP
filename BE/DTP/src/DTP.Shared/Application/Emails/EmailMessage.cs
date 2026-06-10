using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Emails
{
    public class EmailMessage
    {
        public string To { get; set; } = default!;

        public string Subject { get; set; } = default!;

        public string HtmlBody { get; set; } = default!;

        public string? TextBody { get; set; }
    }
}
