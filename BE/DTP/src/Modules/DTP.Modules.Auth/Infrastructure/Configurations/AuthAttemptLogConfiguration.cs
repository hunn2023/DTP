using DTP.Modules.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DTP.Modules.Auth.Infrastructure.Configurations
{
    public class AuthAttemptLogConfiguration : IEntityTypeConfiguration<AuthAttemptLog>
    {
        public void Configure(EntityTypeBuilder<AuthAttemptLog> builder)
        {
            builder.ToTable("AuthAttemptLogs");

            builder.HasKey(x => x.Id);


            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            builder.Property(x => x.ActionType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Identifier)
                .IsRequired();


            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.HasIndex(x => x.IpAddress);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
