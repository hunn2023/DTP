using DTP.Modules.Knowledge.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Abstractions
{
    public interface IKnowledgeSearchService
    {
        Task<List<KnowledgeSearchResultDto>> SearchAsync(
            string query,
            int topK = 5,
            CancellationToken cancellationToken = default);
    }
}
