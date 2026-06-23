using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Services;
using DTP.Modules.Provider.Infrastructure.Clients;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Modules.Provider.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure
{
    public static class ProviderWorkerModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddProviderWorkerModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ProviderDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddScoped<IProviderUnitOfWork, ProviderDbContext>();


            services.AddScoped<IProviderUnitOfWork, ProviderUnitOfWork>();

            // Repository worker cần
            services.AddScoped<IProviderOrderRepository, ProviderOrderRepository>();
            services.AddScoped<IProviderOrderItemRepository, ProviderOrderItemRepository>();
            services.AddScoped<IProviderRedeemRepository, ProviderRedeemRepository>();

            services.AddScoped<IProviderRepository, ProviderRepository>();

            // Service worker cần
            services.AddScoped<IProviderOrderReader, ProviderOrderReader>();
            services.AddScoped<IProviderDeliveryEmailService, ProviderDeliveryEmailService>();
            services.AddScoped<IProviderRedeemPollingService, ProviderRedeemPollingService>();

            // Peacom client
            services.AddHttpClient<IPeacomProviderClient, PeacomProviderClient>((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var baseUrl = config["Providers:Peacom:BaseUrl"];
                var apiKey = config["Providers:Peacom:ApiKey"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("Missing config Providers:Peacom:BaseUrl");

                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new InvalidOperationException("Missing config Providers:Peacom:ApiKey");

                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("apikey", apiKey);
            });

            return services;
        }
    }
}
