using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Modules.Ordering.Infrastructure.Services;
using DTP.Shared.Application.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure
{
    public static class OrderingWorkerModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderingWorkerModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OrderingDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IOrderProviderReader, OrderProviderReader>();
            services.AddScoped<IOrderDeliveryReader, OrderDeliveryReader>();
            return services;
        }
    }
}
