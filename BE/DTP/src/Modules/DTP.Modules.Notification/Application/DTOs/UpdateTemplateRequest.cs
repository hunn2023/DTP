using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.DTOs
{
    public class UpdateTemplateRequest
    {
        public string TitleTemplate { get; set; } = default!;

        public string ContentTemplate { get; set; } = default!;

        public bool IsActive { get; set; }
    }
}
