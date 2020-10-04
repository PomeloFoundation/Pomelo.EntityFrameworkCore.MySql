#!/usr/bin/env pwsh

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")
dotnet tool restore;

try
{
  $targetDirectoryName = "Scaffold"
  $tables = 'DataTypesSimple', 'DataTypesVariable'

  Remove-Item -Recurse -Force $targetDirectoryName -ErrorAction Ignore
  mkdir $targetDirectoryName

  $connectionString = dotnet run connectionString
  $arguments = "ef", "dbcontext", "scaffold", $connectionString, "Pomelo.EntityFrameworkCore.MySql", "--msbuildprojectextensionspath", "../../artifacts/obj/EFCore.MySql.IntegrationTests/", "--output-dir", $targetDirectoryName

  foreach ($table in $tables)
  {
    $arguments += "--table", $table
  }

  & dotnet $arguments

  foreach ($table in $tables)
  {
    $file = Join-Path $targetDirectoryName, $table + '.cs'

    if (!(Test-Path $file -PathType Leaf))
    {
      throw "Failed to scaffold file: $file"
    }
  }
}
finally
{
  Remove-Item -Recurse -Force $targetDirectoryName -ErrorAction Ignore
  Pop-Location
}