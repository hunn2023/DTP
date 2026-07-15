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
    public class KnowledgeDbContext : DbContext, IKnowledgeDbContext
    {
        public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options)
            : base(options)
        {
        }

        public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();

        public DbSet<ProductKnowledgeReadModel> Products => Set<ProductKnowledgeReadModel>();

        public DbSet<ProductFaqKnowledgeReadModel> ProductFaqs => Set<ProductFaqKnowledgeReadModel>();

        public DbSet<ContentKnowledgeReadModel> Contents => Set<ContentKnowledgeReadModel>();

        public DbSet<ContentFaqKnowledgeReadModel> ContentFaqs => Set<ContentFaqKnowledgeReadModel>();

        public DbSet<ProductContentKnowledgeReadModel> ProductContents => Set<ProductContentKnowledgeReadModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KnowledgeDbContext).Assembly);
        }
    }
}
