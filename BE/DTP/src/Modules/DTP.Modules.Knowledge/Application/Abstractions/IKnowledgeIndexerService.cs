using DTP.Modules.Knowledge.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Abstractions
{
    public interface IKnowledgeIndexerService
    {
        Task<ReindexKnowledgeResultDto> ReindexAllAsync(
            CancellationToken cancellationToken = default);
    }
}
