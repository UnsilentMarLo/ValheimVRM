param (
    [string]$Configuration = "Release"
)

# You must set these environment variables locally for the build to find the Valheim and UniVRM DLLs
if (-not $env:VALHEIM_INSTALL) {
    Write-Host "Warning: VALHEIM_INSTALL environment variable is not set." -ForegroundColor Yellow
}
if (-not $env:UNIVRM_UNITY_LIBS) {
    Write-Host "Warning: UNIVRM_UNITY_LIBS environment variable is not set." -ForegroundColor Yellow
}

Write-Host "Building ValheimVRM in $Configuration mode..." -ForegroundColor Cyan

dotnet build ValheimVRM\ValheimVRM.csproj --configuration $Configuration

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully." -ForegroundColor Green
    Write-Host "Output should be in the 'release' directory." -ForegroundColor Green
} else {
    Write-Host "Build failed." -ForegroundColor Red
}
