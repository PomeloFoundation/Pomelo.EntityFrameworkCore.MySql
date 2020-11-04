#!/usr/bin/env pwsh

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")

try
{
  $legacyVersions = Get-ChildItem 'LegacyMigrations' -Directory | % { $_.Name }

  foreach ($legacyVersion in $legacyVersions)
  {
      Remove-Item (Join-Path 'Migrations' '*.cs')
      dotnet ef database drop -f --msbuildprojectextensionspath '../../artifacts/obj/EFCore.MySql.IntegrationTests/'

      Copy-Item (Join-Path 'LegacyMigrations' $legacyVersion '*.csbak') 'Migrations'
      Get-ChildItem (Join-Path 'Migrations' '*.csbak') | % { Rename-Item -Path $_.FullName -NewName ([System.IO.Path]::ChangeExtension($_.Name, ".cs")) }

      dotnet ef migrations add current --msbuildprojectextensionspath '../../artifacts/obj/EFCore.MySql.IntegrationTests/'
      dotnet ef database update --msbuildprojectextensionspath '../../artifacts/obj/EFCore.MySql.IntegrationTests/'
      dotnet test
  }
}
finally
{
  Remove-Item (Join-Path 'Migrations' '*.cs')
  Pop-Location
}
