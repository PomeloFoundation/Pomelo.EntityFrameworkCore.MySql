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
  Write-Host "Starting rebuild process..."
  
  # First, restore packages with error handling
  Write-Host "Restoring packages..."
  dotnet restore --ignore-failed-sources --property WarningsAsErrors= --property TreatWarningsAsErrors=false
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Package restore failed"
    exit $LASTEXITCODE
  }
  
  # Build the project to ensure it compiles
  Write-Host "Building project..."
  dotnet build -c Debug --no-restore
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
  }
  
  # Restore dotnet tools
  Write-Host "Restoring dotnet tools..."
  dotnet tool restore --ignore-failed-sources
  if ($LASTEXITCODE -ne 0) {
    Write-Warning "Tool restore failed, trying alternative approach..."
    
    # Try installing EF Core tools globally as a fallback
    dotnet tool install --global dotnet-ef --ignore-failed-sources
    if ($LASTEXITCODE -ne 0) {
      Write-Warning "Global EF tool install also failed, but continuing..."
    }
  }

  # Remove existing migrations
  Write-Host "Removing existing migrations..."
  if (Test-Path (Join-Path "Migrations" "*.cs")) {
    Remove-Item (Join-Path "Migrations" "*.cs")
  }

  # Drop database
  Write-Host "Dropping database..."
  dotnet ef database drop -f
  if ($LASTEXITCODE -ne 0) {
    Write-Warning "Database drop failed, but continuing..."
  }
  
  # Add initial migration
  Write-Host "Adding Initial migration..."
  dotnet ef migrations add Initial
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Migration add failed"
    exit $LASTEXITCODE
  }

  # add using System.Collections.Generic to the migration files
  Write-Host "Updating migration files..."
  Get-ChildItem (Join-Path "Migrations" "*.cs") | ForEach-Object {
    if (!( select-string -pattern "using System.Collections.Generic;" -path $_.FullName))
    {
      $_.FullName
      "using System.Collections.Generic;" | Insert-Content $_.FullName
    }
  }

  # Update database
  Write-Host "Updating database..."
  dotnet ef database update
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Database update failed"
    exit $LASTEXITCODE
  }
  
  Write-Host "Rebuild process completed successfully!"
}
finally
{
  Pop-Location
}