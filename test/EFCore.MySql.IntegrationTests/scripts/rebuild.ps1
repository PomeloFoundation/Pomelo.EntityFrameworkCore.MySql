#!/usr/bin/env pwsh

function Insert-Content($file)
{
  BEGIN {
    $content = Get-Content $file
  }
  PROCESS {
    $_ | Set-Content $file
  }
  END {
    $content | Add-Content $file
  }
}

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")

try
{
  dotnet tool restore;

  Remove-Item (Join-Path "Migrations" "*.cs")

  dotnet ef database drop -f --msbuildprojectextensionspath '../../artifacts/obj/EFCore.MySql.IntegrationTests/'
  dotnet ef migrations add initial --msbuildprojectextensionspath '../../artifacts/obj/EFCore.MySql.IntegrationTests/'

  # add using System.Collections.Generic to the migration files
  Get-ChildItem (Join-Path "Migrations" "*.cs") | ForEach-Object {
    if (!( select-string -pattern "using System.Collections.Generic;" -path $_.FullName))
    {
      $_.FullName
      "using System.Collections.Generic;" | Insert-Content $_.FullName
    }
  }

  dotnet ef database update --msbuildprojectextensionspath '../../artifacts/obj/EFCore.MySql.IntegrationTests/'
}
finally
{
  Pop-Location
}