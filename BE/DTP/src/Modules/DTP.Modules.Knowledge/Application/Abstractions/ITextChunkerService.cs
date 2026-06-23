using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Abstractions
{
    public interface ITextChunkerService
    {
        IReadOnlyList<string> Split(string text, int maxChars, int overlapChars);
    }
}
