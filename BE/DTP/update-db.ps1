$ErrorActionPreference = "Stop"

function Update-DbContext {
    param (
        [string]$Name,
        [string]$Project,
        [string]$StartupProject,
        [string]$Context
    )

    Write-Host ""
    Write-Host "====================================="
    Write-Host "Updating $Name"
    Write-Host "Project: $Project"
    Write-Host "Context: $Context"
    Write-Host "====================================="

    dotnet ef database update `
      --project $Project `
      --startup-project $StartupProject `
      --context $Context

    if ($LASTEXITCODE -ne 0) {
        throw "Database update failed for $Name"
    }

    Write-Host "$Name updated successfully."
}

Write-Host "====================================="
Write-Host "DTP DATABASE UPDATE STARTED"
Write-Host "====================================="

Update-DbContext `
    -Name "AuthDbContext" `
    -Project "src/Modules/DTP.Modules.Audit/DTP.Modules.Audit.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "AuditDbContext"

Update-DbContext `
    -Name "AuthDbContext" `
    -Project "src/Modules/DTP.Modules.Auth/DTP.Modules.Auth.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "AuthDbContext"

Update-DbContext `
    -Name "CatalogDbContext" `
    -Project "src/Modules/DTP.Modules.Catalog/DTP.Modules.Catalog.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "CatalogDbContext"

Update-DbContext `
    -Name "OrderingDbContext" `
    -Project "src/Modules/DTP.Modules.Content/DTP.Modules.Content.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "ContentDbContext"

Update-DbContext `
    -Name "OrderingDbContext" `
    -Project "src/Modules/DTP.Modules.Delivery/DTP.Modules.Delivery.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "DeliveryDbContext"

Update-DbContext `
    -Name "OrderingDbContext" `
    -Project "src/Modules/DTP.Modules.Ordering/DTP.Modules.Ordering.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "OrderingDbContext"

Update-DbContext `
    -Name "OrderingDbContext" `
    -Project "src/Modules/DTP.Modules.Payment/DTP.Modules.Payment.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "PaymentDbContext"

Update-DbContext `
    -Name "OrderingDbContext" `
    -Project "src/Modules/DTP.Modules.Provider/DTP.Modules.Provider.csproj" `
    -StartupProject "src/DTP.Api/DTP.Api.csproj" `
    -Context "ProviderDbContext"

Write-Host ""
Write-Host "====================================="
Write-Host "DTP DATABASE UPDATE COMPLETED"
Write-Host "====================================="