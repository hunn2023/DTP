using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Modules.Ordering.Infrastructure.Repositories;
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

namespace DTP.Modules.Ordering
{
    public static class OrderingModule
    {
        public static IServiceCollection AddOrderingModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OrderingDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();

            services.AddScoped<IOrderCodeGenerator, OrderCodeGenerator>();

            services.AddScoped<IOrderingCatalogService, OrderingCatalogService>();

            // Tạm thời dùng fake, sau này thay bằng Payment Module thật
            services.AddScoped<IOrderingPaymentService, FakeOrderingPaymentService>();

            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();

            services.AddScoped<IOrderDeliveryReader, OrderDeliveryReader>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IOrderRateLimitService, OrderRateLimitService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(OrderingModule).Assembly);
            });

            return services;
        }
    }
}
