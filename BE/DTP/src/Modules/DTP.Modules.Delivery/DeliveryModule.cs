using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Infrastructure.Persistence;
using DTP.Modules.Delivery.Infrastructure.Repositories;
using DTP.Modules.Delivery.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            services.AddScoped<IEsimProfileRepository, EsimProfileRepository>();
            services.AddScoped<IDigitalDeliveryRepository, DigitalDeliveryRepository>();
            services.AddScoped<IDeliveryUnitOfWork, DeliveryUnitOfWork>();

            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IDeliveryOrderingService, DeliveryOrderingService>();

            // Tạm fake, sau này thay bằng Notification Module
            services.AddScoped<IDeliveryNotificationService, FakeDeliveryNotificationService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DeliveryModule).Assembly);
            });

            return services;
        }
    }
}
