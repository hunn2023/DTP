using DTP.Infrastructure.Email;
using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Infrastructure;
using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Infrastructure;
using DTP.Modules.Ordering;
using DTP.Modules.Ordering.Infrastructure;
using DTP.Modules.Provider;
using DTP.Modules.Provider.Infrastructure;
using DTP.Shared.Application.Emails;
using DTP.WorkerService.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DTP.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<ProviderRedeemPollingWorker>();

            builder.Services.Configure<ProviderRedeemWorkerOptions>(
            builder.Configuration.GetSection("Workers:ProviderRedeem"));

            builder.Services.AddAuditWorkerModule(builder.Configuration);
            builder.Services.AddOrderingWorkerModule(builder.Configuration);
            builder.Services.AddProviderWorkerModule(builder.Configuration);
            builder.Services.AddDeliveryWorkerModule(builder.Configuration);

            builder.Services.Replace(
    ServiceDescriptor.Scoped<ICurrentAuditUserService, WorkerCurrentAuditUserService>());

            builder.Services.Replace(
    ServiceDescriptor.Scoped<IDeliveryRateLimitService, WorkerDeliveryRateLimitService>());

            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            var host = builder.Build();
            host.Run();
        }
    }
}