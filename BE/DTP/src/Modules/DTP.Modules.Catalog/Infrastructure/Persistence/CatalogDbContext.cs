using DTP.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Catalog.Infrastructure.Persistence
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Carrier> Carriers => Set<Carrier>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

        public DbSet<ProductVariantFeature> ProductVariantFeatures => Set<ProductVariantFeature>();

        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();

        public DbSet<Provider> Providers => Set<Provider>();

        public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();


        public DbSet<EsimPackage> EsimPackages => Set<EsimPackage>();

        public DbSet<PhoneCard> PhoneCards => Set<PhoneCard>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(CatalogDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
