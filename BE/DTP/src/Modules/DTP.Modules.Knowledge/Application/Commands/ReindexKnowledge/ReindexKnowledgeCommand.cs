using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Commands.ReindexKnowledge
{
    public record ReindexKnowledgeCommand : IRequest<ReindexKnowledgeResultDto>;

    public class ReindexKnowledgeCommandHandler
    : IRequestHandler<ReindexKnowledgeCommand, ReindexKnowledgeResultDto>
    {
        private readonly IKnowledgeIndexerService _indexerService;

        public ReindexKnowledgeCommandHandler(IKnowledgeIndexerService indexerService)
        {
            _indexerService = indexerService;
        }

        public Task<ReindexKnowledgeResultDto> Handle(
            ReindexKnowledgeCommand request,
            CancellationToken cancellationToken)
        {
            return _indexerService.ReindexAllAsync(cancellationToken);
        }
    }
}
