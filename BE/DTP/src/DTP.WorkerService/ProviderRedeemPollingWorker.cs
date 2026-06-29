using DTP.Modules.Provider.Application.Abstractions.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.WorkerService
{
    public class ProviderRedeemPollingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProviderRedeemPollingWorker> _logger;
        private readonly ProviderRedeemWorkerOptions _options;

        public ProviderRedeemPollingWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<ProviderRedeemPollingWorker> logger,
            IOptions<ProviderRedeemWorkerOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("ProviderRedeemPollingWorker is disabled.");
                return;
            }

            _logger.LogInformation(
                "ProviderRedeemPollingWorker started. Interval={Interval}s, PollBatch={PollBatch}, EmailBatch={EmailBatch}",
                _options.PollIntervalSeconds,
                _options.PollBatchSize,
                _options.EmailBatchSize);

            if (_options.InitialDelaySeconds > 0)
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(_options.InitialDelaySeconds),
                    stoppingToken);
            }

            using var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(_options.PollIntervalSeconds));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "ProviderRedeemPollingWorker run failed.");
                }

                try
                {
                    await timer.WaitForNextTickAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("ProviderRedeemPollingWorker stopped.");
        }

        private async Task RunOnceAsync( CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var pollingService = scope.ServiceProvider
                .GetRequiredService<IProviderRedeemPollingService>();

            try
            {
                _logger.LogInformation("ProviderRedeemPollingWorker polling pending redeems...");

                await pollingService.PollPendingRedeemsAsync(
                    take: _options.PollBatchSize,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ProviderRedeemPollingWorker poll pending redeems failed.");
            }

            try
            {
                _logger.LogInformation("ProviderRedeemPollingWorker processing ready deliveries...");

                await pollingService.SendDoneRedeemEmailsAsync(
                    take: _options.EmailBatchSize,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ProviderRedeemPollingWorker process ready deliveries failed.");
            }

            _logger.LogInformation("ProviderRedeemPollingWorker run completed.");
        }
    }
}
