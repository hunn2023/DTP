using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.Options;
using DTP.Modules.Payment.Infrastructure.Clients;
using DTP.Modules.Payment.Infrastructure.Persistence;
using DTP.Modules.Payment.Infrastructure.Repositories;
using DTP.Modules.Payment.Infrastructure.Services;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;


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

            services.Configure<VnptEpayOptions>(
                 configuration.GetSection("Payment:VnptEpay"));
            services.AddScoped<IPaymentRealtimeNotifier, PaymentRealtimeNotifier>();

            services.AddHttpClient<IVnptEpayClient, VnptEpayClient>((sp, client) =>
            {
                var options = configuration
                    .GetSection("Payment:VnptEpay")
                    .Get<VnptEpayOptions>()!;

                client.BaseAddress = new Uri("1212121");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.Configure<SepayOptions>(
    configuration.GetSection("Payment:Sepay"));

            services.AddScoped<ISepayWebhookVerifier, SepayWebhookVerifier>();
             
            // Repositories
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IPaymentCallbackLogRepository, PaymentCallbackLogRepository>();
            services.AddScoped<IPaymentUnitOfWork, PaymentUnitOfWork>();

            // Services
            services.AddScoped<IPaymentService, PaymentService>(); 
            services.AddScoped<IVnptEpayClient, VnptEpayClient>();
            services.AddScoped<IPaymentAuditService, PaymentAuditService>();

            services.AddScoped<IOrderPaymentService, OrderPaymentService>();
            services.AddScoped<IPaymentRateLimitService, PaymentRateLimitService>();

            services.AddScoped<IProviderReservationService, ProviderReservationService>();
            services.AddScoped<ISepayPaymentService, SepayPaymentService>();

            services.AddScoped<IPaymentUnitOfWork, PaymentUnitOfWork>();

            services.AddScoped<IPaymentProviderRepository, PaymentProviderRepository>();

            services.AddScoped<IPaymentProviderService, PaymentProviderService>();



            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(PaymentModule).Assembly);
            });

            return services;
        }
    }
}
