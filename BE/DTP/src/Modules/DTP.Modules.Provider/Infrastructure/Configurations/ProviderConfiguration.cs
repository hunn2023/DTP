using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Configurations
{
    public class ProviderConfiguration
      : IEntityTypeConfiguration<Domain.Entities.Provider>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Provider> builder)
        {
            builder.ToTable("Providers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ProviderType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.BaseUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.ApiKey)
                .HasMaxLength(1000);

            builder.Property(x => x.SecretKey)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}
