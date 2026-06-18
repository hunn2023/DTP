using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Abstractions
{
    public interface IEmbeddingService
    {
        Task<float[]> CreateEmbeddingAsync(
            string input,
            CancellationToken cancellationToken = default);
    }
}
