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

            services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            services.AddScoped<IDeliveryUnitOfWork, DeliveryUnitOfWork>();

            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IEsimDeliveryEmailService, EsimDeliveryEmailService>();
            services.AddScoped<IDigitalFulfillmentService, MockDigitalFulfillmentService>();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DeliveryModule).Assembly);
            });

            return services;
        }
    }
}
