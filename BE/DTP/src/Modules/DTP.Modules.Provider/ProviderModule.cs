using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Gateways;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Services;
using DTP.Modules.Provider.Infrastructure;
using DTP.Modules.Provider.Infrastructure.Gateways;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Modules.Provider.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer; // Add this using directive

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

            services.AddScoped<IProviderUnitOfWork, ProviderUnitOfWork>();

            services.AddScoped<IExternalProviderRepository, ExternalProviderRepository>();
            services.AddScoped<IProviderCredentialRepository, ProviderCredentialRepository>();
            services.AddScoped<IProviderProductMappingRepository, ProviderProductMappingRepository>();
            services.AddScoped<IProviderOrderRepository, ProviderOrderRepository>();
            services.AddScoped<IProviderApiLogRepository, ProviderApiLogRepository>();
            services.AddScoped<IProviderWebhookLogRepository, ProviderWebhookLogRepository>();

            services.AddScoped<IProviderGatewayFactory, ProviderGatewayFactory>();
            services.AddScoped<FakeProviderGateway>();

            services.AddScoped<IProviderProvisioningService, ProviderProvisioningService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ProviderModule).Assembly);
            });

            return services;
        }
    }
}
