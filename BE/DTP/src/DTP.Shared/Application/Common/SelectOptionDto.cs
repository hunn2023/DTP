using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class SelectOptionDto
    {
        public Guid Id { get; set; }

        public string Label { get; set; } = default!;

        public string Value { get; set; } = default!;

        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;

        public SelectOptionDto()
        {
        }

        public SelectOptionDto(Guid id, string label)
        {
            Id = id;
            Label = label;
            Value = id.ToString();
        }

        public SelectOptionDto(Guid id, string label, string? code)
        {
            Id = id;
            Label = label;
            Value = id.ToString();
            Code = code;
        }
    }
}
