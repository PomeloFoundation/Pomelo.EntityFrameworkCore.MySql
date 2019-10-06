#!/usr/bin/env powershell

. $PSScriptRoot\eng\common\tools.ps1
InitializeDotNetCli -install:$true | Out-Null
$env:DOTNET_ROOT = $env:DOTNET_INSTALL_DIR

# bug where PATH is semicolon separated on Linux
if (!$IsWindows) {
    $env:PATH = $env:PATH.Replace(";", ":")
}

& ($ARGS | Select-Object -First 1) @($ARGS | Select-Object -Skip 1)
