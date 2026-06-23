using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.DTOs;
using DTP.Modules.Knowledge.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Commands.ReindexKnowledge
{
    public record ReindexKnowledgeSourceCommand(
      KnowledgeSourceType SourceType,
      Guid SourceId) : IRequest<ReindexKnowledgeResultDto>;


    public class ReindexKnowledgeSourceCommandHandler
    : IRequestHandler<ReindexKnowledgeSourceCommand, ReindexKnowledgeResultDto>
    {
        private readonly IKnowledgeIndexerService _indexerService;

        public ReindexKnowledgeSourceCommandHandler(IKnowledgeIndexerService indexerService)
        {
            _indexerService = indexerService;
        }

        public Task<ReindexKnowledgeResultDto> Handle(
            ReindexKnowledgeSourceCommand request,
            CancellationToken cancellationToken)
        {
            return _indexerService.ReindexSourceAsync(
                request.SourceType,
                request.SourceId,
                cancellationToken);
        }
    }

}
