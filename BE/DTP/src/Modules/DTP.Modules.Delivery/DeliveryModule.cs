using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Infrastructure.Persistence;
using DTP.Modules.Delivery.Infrastructure.Repositories;
using DTP.Modules.Delivery.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Delivery
{
    public static class DeliveryModule
    {
        public static IServiceCollection AddDeliveryModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<DeliveryDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            services.AddScoped<IDeliveryUnitOfWork, DeliveryUnitOfWork>();

            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IEsimDeliveryEmailService, EsimDeliveryEmailService>();
            services.AddScoped<IDigitalFulfillmentService, MockDigitalFulfillmentService>();
            services.AddScoped<IDeliveryRateLimitService, DeliveryRateLimitService>();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DeliveryModule).Assembly);
            });

            return services;
        }
    }
}
