using DTP.Modules.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Persistence
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContentPage> ContentPages => Set<ContentPage>();
        public DbSet<ContentArticle> ContentArticles => Set<ContentArticle>();
        public DbSet<ContentBanner> ContentBanners => Set<ContentBanner>();
        public DbSet<ContentFaq> ContentFaqs => Set<ContentFaq>();
        public DbSet<SeoMetadata> SeoMetadata => Set<SeoMetadata>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentDbContext).Assembly);
        }
    }
}
