$ErrorActionPreference = "Stop"

function DownloadWithRetry([string] $url, [string] $downloadLocation, [int] $retries) 
{
    while($true)
    {
        try
        {
            Invoke-WebRequest $url -OutFile $downloadLocation
            break
        }
        catch
        {
            $exceptionMessage = $_.Exception.Message
            Write-Host "Failed to download '$url': $exceptionMessage"
            if ($retries -gt 0) {
                $retries--
                Write-Host "Waiting 10 seconds before retrying. Retries left: $retries"
                Start-Sleep -Seconds 10

            }
            else 
            {
                $exception = $_.Exception
                throw $exception
            }
        }
    }
}

cd $PSScriptRoot

$repoFolder = $PSScriptRoot
$env:REPO_FOLDER = $repoFolder

$koreBuildZip="https://github.com/aspnet/KoreBuild/archive/dev.zip"
if ($env:KOREBUILD_ZIP)
{
    $koreBuildZip=$env:KOREBUILD_ZIP
}

$buildFolder = ".build"

if (!(Test-Path $buildFolder)) {
    Write-Host "Downloading KoreBuild from $koreBuildZip"    
    
    $tempFolder=$env:TEMP + "\KoreBuild-" + [guid]::NewGuid()
    New-Item -Path "$tempFolder" -Type directory | Out-Null

    $localZipFile="$tempFolder\korebuild.zip"
    
    DownloadWithRetry -url $koreBuildZip -downloadLocation $localZipFile -retries 6

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($localZipFile, $tempFolder)
    
    New-Item -Path "$buildFolder" -Type directory | Out-Null
    copy-item "$tempFolder\**\build\*" $buildFolder -Recurse

    # Cleanup
    if (Test-Path $tempFolder) {
        Remove-Item -Recurse -Force $tempFolder
    }
}

$dotnetVersionFile = $buildFolder + "\cli.version.win"
$dotnetChannel = "preview"
$dotnetVersion = Get-Content $dotnetVersionFile
$dotnetSharedRuntimeVersion = "1.0.0"
$dotnetSharedRuntimeChannel = "preview"

$dotnetLocalInstallFolder = $env:DOTNET_INSTALL_DIR
if (!$dotnetLocalInstallFolder)
{
    $dotnetLocalInstallFolder = "$env:LOCALAPPDATA\Microsoft\dotnet\"
}

& "$buildFolder\dotnet\dotnet-install.ps1" -Channel $dotnetChannel -Version $dotnetVersion -Architecture x64
# Avoid redownloading the CLI if it's already installed.
$sharedRuntimePath = [IO.Path]::Combine($dotnetLocalInstallFolder, 'shared', 'Microsoft.NETCore.App', $dotnetSharedRuntimeVersion)
if (!(Test-Path $sharedRuntimePath))
{
    & "$buildFolder\dotnet\dotnet-install.ps1" -Channel $dotnetSharedRuntimeChannel -SharedRuntime -Version $dotnetSharedRuntimeVersion -Architecture x64
}

##########################
# BEGIN REPOSITORY TESTS
##########################

# restore
cd "$repoFolder"
& dotnet restore
if ($LASTEXITCODE -ne 0){
    exit $LASTEXITCODE;
}

# build .NET 451 to verify no build errors
cd (Join-Path $repoFolder (Join-Path "src" "Pomelo.EntityFrameworkCore.MySql"))
& dotnet build -c Release
if ($LASTEXITCODE -ne 0){
    exit $LASTEXITCODE;
}

# run unit tests
cd (Join-Path $repoFolder (Join-Path "test" "Pomelo.EntityFrameworkCore.MySql.Tests"))
& dotnet test -c Release
if ($LASTEXITCODE -ne 0){
    exit $LASTEXITCODE;
}

# run functional tests if not on MyGet
if ($env:BuildRunner -ne "MyGet"){
    cd (Join-Path $repoFolder (Join-Path "test" "Pomelo.EntityFrameworkCore.MySql.PerfTests"))
    & dotnet test -c Release
    if ($LASTEXITCODE -ne 0){
        exit $LASTEXITCODE;
    }
}
