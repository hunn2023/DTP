using DTP.Modules.Provider;

namespace DTP.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<ProviderRedeemWorkerOptions>(
    builder.Configuration.GetSection("Workers:ProviderRedeem"));

        builder.Services.AddProviderModule(builder.Configuration);

        builder.Services.AddHostedService<ProviderRedeemPollingWorker>();
        var host = builder.Build();
        host.Run();
    }
}