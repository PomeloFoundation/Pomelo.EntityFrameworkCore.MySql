#!/usr/bin/env pwsh

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")

try
{
  C:\'Program Files'\dotnet\dotnet.exe tool restore;

  $targetDirectoryName = "Scaffold"
  $tables = 'DataTypesSimple', 'DataTypesVariable'

  Remove-Item -Recurse -Force $targetDirectoryName -ErrorAction Ignore
  mkdir $targetDirectoryName

  $connectionString = (Select-String config.json -Pattern '(?<="ConnectionString":\s*")(.*?)(?=")').Matches.Value
  $arguments = "ef", "dbcontext", "scaffold", $connectionString, "Pomelo.EntityFrameworkCore.MySql", "--output-dir", $targetDirectoryName

  foreach ($table in $tables)
  {
    $arguments += "--table", $table
  }

  & C:\'Program Files'\dotnet\dotnet.exe $arguments

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