using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Services;
using DTP.Modules.Provider.Infrastructure;
using DTP.Modules.Provider.Infrastructure.Clients;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Modules.Provider.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace DTP.Modules.Provider
{
    public static class ProviderModule
    {
        public static IServiceCollection AddProviderModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ProviderDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHttpClient<IPeacomProviderClient, PeacomProviderClient>(client =>
            {
                client.BaseAddress = new Uri("https://dev-api.peacom.co");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddScoped<IProviderUnitOfWork, ProviderUnitOfWork>();

            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IProviderPackageProductRepository, ProviderPackageProductRepository>();
            services.AddScoped<IProviderProductMappingRepository, ProviderProductMappingRepository>();
            services.AddScoped<IProviderApiLogRepository, ProviderApiLogRepository>();
            services.AddScoped<IProviderFulfillmentLogRepository, ProviderFulfillmentLogRepository>();

            services.AddScoped<IProviderPackageSyncService, ProviderPackageSyncService>();
            services.AddScoped<IProviderFulfillmentService, ProviderFulfillmentService>();
            services.AddScoped<IProviderOrderRepository, ProviderOrderRepository>();
            services.AddScoped<IProviderOrderItemRepository, ProviderOrderItemRepository>();

            services.AddHttpClient<IPeacomProviderClient, PeacomProviderClient>();

            services.AddScoped<IProviderOrderReader, ProviderOrderReader>();
            services.AddScoped<IProviderRedeemRepository, ProviderRedeemRepository>();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ProviderModule).Assembly);
            });

            return services;
        }
    }
}
