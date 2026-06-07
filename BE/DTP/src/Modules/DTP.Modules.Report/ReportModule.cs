using DTP.Modules.Report.Application.Abstractions.Caching;
using DTP.Modules.Report.Application.Abstractions.Exports;
using DTP.Modules.Report.Application.Abstractions.Persistence;
using DTP.Modules.Report.Application.Abstractions.Repositories;
using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.Services;
using DTP.Modules.Report.Infrastructure.Caching;
using DTP.Modules.Report.Infrastructure.Exports;
using DTP.Modules.Report.Infrastructure.Persistence;
using DTP.Modules.Report.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Report
{
    public static class ReportModule
    {
        public static IServiceCollection AddReportModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ReportReadDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IReportReadDbContext>(provider =>
                provider.GetRequiredService<ReportReadDbContext>());

            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportExportService, CsvReportExportService>();
            services.AddScoped<IReportCacheInvalidator, ReportCacheInvalidator>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ReportModule).Assembly);
            });

            return services;
        }
    }
}
