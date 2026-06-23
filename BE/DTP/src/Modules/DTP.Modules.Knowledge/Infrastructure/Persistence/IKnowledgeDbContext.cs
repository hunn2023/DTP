using DTP.Modules.Knowledge.Domain.Entities;
using DTP.Modules.Knowledge.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence
{
    public interface IKnowledgeDbContext
    {
        DbSet<KnowledgeChunk> KnowledgeChunks { get; }

        DbSet<ProductKnowledgeReadModel> Products { get; }

        DbSet<ProductFaqKnowledgeReadModel> ProductFaqs { get; }

        DbSet<ContentKnowledgeReadModel> Contents { get; }

        DbSet<ContentFaqKnowledgeReadModel> ContentFaqs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
