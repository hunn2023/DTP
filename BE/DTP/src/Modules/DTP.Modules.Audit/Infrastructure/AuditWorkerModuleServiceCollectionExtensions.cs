using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.Services;
using DTP.Modules.Audit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Infrastructure
{
    public static class AuditWorkerModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditWorkerModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AuditDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IAuditLogWriter, AuditLogWriter>();

            return services;
        }
    }
}
