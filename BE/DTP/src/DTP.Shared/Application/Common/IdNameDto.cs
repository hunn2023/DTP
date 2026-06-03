using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class IdNameDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public IdNameDto()
        {
        }

        public IdNameDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
