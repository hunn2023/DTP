using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class DeleteRequestDto
    {
        public List<Guid> Ids { get; set; } = new();
    }
}
