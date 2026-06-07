using DTP.Modules.Audit.Application.Abstractions.Repositories;
using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.Abstractions.UnitOfWork;
using DTP.Modules.Audit.Application.Services;
using DTP.Modules.Audit.Infrastructure.Persistence;
using DTP.Modules.Audit.Infrastructure.Repositories;
using DTP.Modules.Audit.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Audit
{
    public static class AuditModule
    {
        public static IServiceCollection AddAuditModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AuditDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IAuditUnitOfWork, AuditUnitOfWork>();

            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IAuditLogWriter, AuditLogWriter>();
            services.AddScoped<ICurrentAuditUserService, CurrentAuditUserService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AuditModule).Assembly);
            });

            return services;
        }
    }
}
