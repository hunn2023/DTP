using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Infrastructure.Persistence;
using DTP.Modules.Payment.Infrastructure.Repositories;
using DTP.Modules.Payment.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment
{
    public static class PaymentModule
    {
        public static IServiceCollection AddPaymentModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<PaymentDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IPaymentUnitOfWork, PaymentUnitOfWork>();

            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IVnptEpayClient, FakeVnptEpayClient>();

            services.AddScoped<IPaymentOrderingService, PaymentOrderingService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(PaymentModule).Assembly);
            });

            return services;
        }
    }
}
