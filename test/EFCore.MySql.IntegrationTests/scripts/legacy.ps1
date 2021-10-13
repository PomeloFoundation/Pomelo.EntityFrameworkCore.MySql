#!/usr/bin/env pwsh

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")

try
{
  C:\Program Files\dotnet\dotnet.exe tool restore;

  $legacyVersions = Get-ChildItem 'LegacyMigrations' -Directory | % { $_.Name }

  foreach ($legacyVersion in $legacyVersions)
  {
      Remove-Item (Join-Path 'Migrations' '*.cs')
      C:\'Program Files'\dotnet\dotnet.exe ef database drop -f

      Copy-Item (Join-Path 'LegacyMigrations' $legacyVersion '*.csbak') 'Migrations'
      Get-ChildItem (Join-Path 'Migrations' '*.csbak') | % { Rename-Item -Path $_.FullName -NewName ([System.IO.Path]::ChangeExtension($_.Name, ".cs")) }

     C:\'Program Files'\dotnet\dotnet.exe ef migrations add current --verbose
    C:\'Program Files'\dotnet\dotnet.exe ef database update --verbose
     C:\'Program Files'\dotnet\dotnet.exe test
      break;
  }
}
finally
{
  # Remove-Item (Join-Path 'Migrations' '*.cs')
  Pop-Location
}
