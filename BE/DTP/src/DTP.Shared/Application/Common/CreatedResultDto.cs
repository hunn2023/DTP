using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class CreatedResultDto
    {
        public Guid Id { get; set; }

        public CreatedResultDto()
        {
        }

        public CreatedResultDto(Guid id)
        {
            Id = id;
        }
    }
}
