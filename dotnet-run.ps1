if (!$DotNetHome) {
    $DotNetHome = if ($env:DOTNET_HOME) { $env:DOTNET_HOME } `
        elseif ($env:USERPROFILE) { Join-Path $env:USERPROFILE '.dotnet'} `
        elseif ($env:HOME) {Join-Path $env:HOME '.dotnet'}`
        else { Join-Path $PSScriptRoot '.dotnet'}
}

Write-Host "DotNetHome = $DotNetHome"

$env:Path = "$DotNetHome;$env:Path"
& ($ARGS | Select-Object -First 1) @($ARGS | Select-Object -Skip 1)
