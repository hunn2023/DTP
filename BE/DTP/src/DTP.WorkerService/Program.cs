using DTP.Modules.Provider;

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

            builder.Services.AddProviderModule(builder.Configuration);

            var host = builder.Build();
            host.Run();
        }
    }
}