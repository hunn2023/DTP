using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Queries
{
    public record SearchKnowledgeQuery(
       string Query,
       int TopK = 5) : IRequest<List<KnowledgeSearchResultDto>>;


    public class SearchKnowledgeQueryHandler
    : IRequestHandler<SearchKnowledgeQuery, List<KnowledgeSearchResultDto>>
    {
        private readonly IKnowledgeSearchService _knowledgeSearchService;

        public SearchKnowledgeQueryHandler(IKnowledgeSearchService knowledgeSearchService)
        {
            _knowledgeSearchService = knowledgeSearchService;
        }

        public Task<List<KnowledgeSearchResultDto>> Handle(
            SearchKnowledgeQuery request,
            CancellationToken cancellationToken)
        {
            return _knowledgeSearchService.SearchAsync(
                request.Query,
                request.TopK,
                cancellationToken);
        }
    }
}
