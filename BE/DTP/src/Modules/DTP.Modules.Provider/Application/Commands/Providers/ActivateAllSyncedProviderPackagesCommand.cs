using DTP.Modules.Provider.Application.Abstractions.Repositories;
using MediatR;

namespace DTP.Modules.Provider.Application.Commands.Providers;

public sealed class ActivateAllSyncedProviderPackagesCommand
    : IRequest<ActivateAllSyncedProviderPackagesResult>
{
    public string ProviderCode { get; set; } = default!;
}

public sealed class ActivateAllSyncedProviderPackagesResult
{
    public int Total { get; set; }
    public int Activated { get; set; }
    public int Failed { get; set; }
    public List<string> Errors { get; set; } = [];
}

public sealed class ActivateAllSyncedProviderPackagesCommandHandler
    : IRequestHandler<
        ActivateAllSyncedProviderPackagesCommand,
        ActivateAllSyncedProviderPackagesResult>
{
    private readonly IProviderRepository _providerRepository;
    private readonly IProviderPackageProductRepository _packageRepository;
    private readonly ISender _sender;

    public ActivateAllSyncedProviderPackagesCommandHandler(
        IProviderRepository providerRepository,
        IProviderPackageProductRepository packageRepository,
        ISender sender)
    {
        _providerRepository = providerRepository;
        _packageRepository = packageRepository;
        _sender = sender;
    }

    public async Task<ActivateAllSyncedProviderPackagesResult> Handle(
        ActivateAllSyncedProviderPackagesCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProviderCode))
            throw new ArgumentException("ProviderCode không được rỗng.");

        var provider = await _providerRepository.GetByCodeAsync(
            request.ProviderCode,
            cancellationToken);

        if (provider is null)
        {
            throw new InvalidOperationException(
                $"Không tìm thấy provider '{request.ProviderCode.Trim()}'.");
        }

        var packages = await _packageRepository.GetByStatusAsync(
            provider.Id,
            "Provisioned",
            cancellationToken);

        var result = new ActivateAllSyncedProviderPackagesResult
        {
            Total = packages.Count
        };

        foreach (var package in packages)
        {
            try
            {
                await _sender.Send(
                    new ActivateSyncedProviderPackageCommand
                    {
                        ProviderPackageProductId = package.Id
                    },
                    cancellationToken);

                result.Activated++;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                result.Failed++;
                result.Errors.Add(
                    $"{package.ProviderSku}: {exception.Message}");
            }
        }

        return result;
    }
}
