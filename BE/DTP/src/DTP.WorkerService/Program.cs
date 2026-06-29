using DTP.Infrastructure.Email;
using DTP.Modules.Delivery.Infrastructure;
using DTP.Modules.Ordering;
using DTP.Modules.Ordering.Infrastructure;
using DTP.Modules.Provider;
using DTP.Modules.Provider.Infrastructure;
using DTP.Shared.Application.Emails;

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

            builder.Services.AddOrderingWorkerModule(builder.Configuration);
            builder.Services.AddProviderWorkerModule(builder.Configuration);
            builder.Services.AddDeliveryWorkerModule(builder.Configuration);

            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            var host = builder.Build();
            host.Run();
        }
    }
}