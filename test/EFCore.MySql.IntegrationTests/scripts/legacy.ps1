#!/usr/bin/env pwsh

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")

try
{
  dotnet tool restore;

  $legacyVersions = Get-ChildItem 'LegacyMigrations' -Directory | % { $_.Name }

  foreach ($legacyVersion in $legacyVersions)
  {
      Remove-Item (Join-Path 'Migrations' '*.cs')
      dotnet ef database drop -f

      Copy-Item (Join-Path 'LegacyMigrations' $legacyVersion '*.csbak') 'Migrations'
      Get-ChildItem (Join-Path 'Migrations' '*.csbak') | % { Rename-Item -Path $_.FullName -NewName ([System.IO.Path]::ChangeExtension($_.Name, ".cs")) }

      dotnet ef migrations add current --verbose
      dotnet ef database update --verbose
      dotnet test
      break;
  }
}
finally
{
  # Remove-Item (Join-Path 'Migrations' '*.cs')
  Pop-Location
}
