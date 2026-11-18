#!/usr/bin/env pwsh
# Script to aggregate benchmark results and update DatabaseVersions.md
# Usage: ./aggregate-benchmark-results.ps1 -ArtifactsDir <artifacts-directory> -OutputFile <output-file>

param(
    [Parameter(Mandatory=$true)]
    [string]$ArtifactsDir,
    
    [Parameter(Mandatory=$true)]
    [string]$OutputFile
)

Write-Host "Aggregating benchmark results from: $ArtifactsDir"
Write-Host "Output file: $OutputFile"
Write-Host ""

# Function to parse BenchmarkDotNet CSV report and extract mean times
function Get-BenchmarkMeanTime {
    param(
        [string]$CsvFile
    )
    
    if (-not (Test-Path $CsvFile)) {
        Write-Host "  Warning: File not found: $CsvFile"
        return $null
    }
    
    try {
        # Import CSV properly to handle quoted fields and column names
        $csvData = Import-Csv -Path $CsvFile
        
        if ($csvData.Count -eq 0) {
            Write-Host "  Warning: No data in CSV: $CsvFile"
            return $null
        }
        
        $meanTimes = @()
        $totalRows = 0
        $validRows = 0
        $naRows = 0
        
        # Check which column name contains mean timing data
        # BenchmarkDotNet uses "Mean" column for average execution time
        $meanColumn = $null
        $properties = $csvData[0].PSObject.Properties.Name
        
        foreach ($prop in $properties) {
            if ($prop -match '^Mean$') {
                $meanColumn = $prop
                break
            }
        }
        
        if (-not $meanColumn) {
            Write-Host "  Warning: 'Mean' column not found in CSV: $CsvFile"
            return $null
        }
        
        # Extract mean times from all benchmarks
        foreach ($row in $csvData) {
            $totalRows++
            $meanValue = $row.$meanColumn
            
            # Check for NA values (benchmark didn't complete or failed)
            if ($meanValue -eq 'NA' -or $meanValue -eq 'N/A' -or [string]::IsNullOrWhiteSpace($meanValue)) {
                $naRows++
                continue
            }
            
            # Try to parse the mean value, handling different formats
            # BenchmarkDotNet may include units like "123.45 ns" or just "123.45"
            if ($meanValue -match '([\d.]+)\s*(ns|μs|ms|s)?') {
                $numericValue = [double]$matches[1]
                $unit = $matches[2]
                
                # Convert to milliseconds based on unit
                $meanMs = switch ($unit) {
                    'ns' { $numericValue / 1000000.0 }
                    'μs' { $numericValue / 1000.0 }
                    'us' { $numericValue / 1000.0 }
                    'ms' { $numericValue }
                    's'  { $numericValue * 1000.0 }
                    default { $numericValue / 1000000.0 }  # Assume nanoseconds if no unit
                }
                
                $meanTimes += $meanMs
                $validRows++
            }
        }
        
        # Provide feedback about data quality
        if ($totalRows -gt 0) {
            Write-Host "  Processed $totalRows benchmarks: $validRows valid, $naRows with NA/missing data"
        }
        
        if ($meanTimes.Count -gt 0) {
            $avgTime = ($meanTimes | Measure-Object -Average).Average
            return [math]::Round($avgTime, 2)
        }
        else {
            Write-Host "  Warning: No valid timing data found (all values are NA or invalid)"
            return $null
        }
    }
    catch {
        Write-Host "  Error parsing $CsvFile : $_"
    }
    
    return $null
}

# Function to extract times for a specific database version and OS
function Get-BenchmarkTimesForVersion {
    param(
        [string]$Version,
        [string]$Os,
        [string]$ArtifactsPath
    )
    
    $dbType = if ($Version -match 'mysql') { 'mysql' } else { 'mariadb' }
    $artifactName = "benchmark-results-$Version-$Os"
    $artifactPath = Join-Path $ArtifactsPath $artifactName
    
    Write-Host "Looking for artifacts in: $artifactPath"
    
    if (-not (Test-Path $artifactPath)) {
        Write-Host "  Warning: Artifact directory not found for $Version on $Os"
        return @{
            Insert = "N/A"
            Update = "N/A"
            Query = "N/A"
        }
    }
    
    $resultsPath = Join-Path $artifactPath "results"
    if (-not (Test-Path $resultsPath)) {
        Write-Host "  Warning: Results directory not found"
        return @{
            Insert = "N/A"
            Update = "N/A"
            Query = "N/A"
        }
    }
    
    # Find CSV files for each benchmark category
    $insertCsv = Get-ChildItem -Path $resultsPath -Filter "*InsertBenchmarks*report.csv" -ErrorAction SilentlyContinue | Select-Object -First 1
    $updateCsv = Get-ChildItem -Path $resultsPath -Filter "*UpdateBenchmarks*report.csv" -ErrorAction SilentlyContinue | Select-Object -First 1
    $queryCsv = Get-ChildItem -Path $resultsPath -Filter "*QueryBenchmarks*report.csv" -ErrorAction SilentlyContinue | Select-Object -First 1
    
    $insertTime = if ($insertCsv) { Get-BenchmarkMeanTime -CsvFile $insertCsv.FullName } else { $null }
    $updateTime = if ($updateCsv) { Get-BenchmarkMeanTime -CsvFile $updateCsv.FullName } else { $null }
    $queryTime = if ($queryCsv) { Get-BenchmarkMeanTime -CsvFile $queryCsv.FullName } else { $null }
    
    return @{
        Insert = if ($insertTime) { $insertTime.ToString() } else { "N/A" }
        Update = if ($updateTime) { $updateTime.ToString() } else { "N/A" }
        Query = if ($queryTime) { $queryTime.ToString() } else { "N/A" }
    }
}

# Read the original DatabaseVersions.md
$content = Get-Content $OutputFile -Raw

# Define database versions and their platforms based on the workflow
$versions = @(
    @{ Version = "9.5.0-mysql"; Ubuntu = $true; Windows = $false }
    @{ Version = "9.4.0-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "9.3.0-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "9.2.0-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "9.1.0-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "9.0.1-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "8.4.3-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "8.0.40-mysql"; Ubuntu = $true; Windows = $true }
    @{ Version = "12.1.2-mariadb"; Ubuntu = $true; Windows = $false }
    @{ Version = "12.0.2-mariadb"; Ubuntu = $true; Windows = $false }
    @{ Version = "11.8.5-mariadb"; Ubuntu = $true; Windows = $false }
    @{ Version = "11.7.2-mariadb"; Ubuntu = $true; Windows = $false }
    @{ Version = "11.7.1-mariadb"; Ubuntu = $false; Windows = $true }
    @{ Version = "11.6.2-mariadb"; Ubuntu = $true; Windows = $true }
    @{ Version = "11.5.2-mariadb"; Ubuntu = $true; Windows = $true }
    @{ Version = "11.4.4-mariadb"; Ubuntu = $true; Windows = $true }
    @{ Version = "11.3.2-mariadb"; Ubuntu = $true; Windows = $true }
    @{ Version = "10.11.10-mariadb"; Ubuntu = $true; Windows = $true }
    @{ Version = "10.6.20-mariadb"; Ubuntu = $true; Windows = $true }
    @{ Version = "10.5.27-mariadb"; Ubuntu = $true; Windows = $true }
)

# Process each version and update times
foreach ($versionInfo in $versions) {
    $version = $versionInfo.Version
    $versionNumber = $version -replace '-mysql|-mariadb', ''
    
    Write-Host ""
    Write-Host "Processing $version..."
    
    # Get Ubuntu times if available
    if ($versionInfo.Ubuntu) {
        $ubuntuTimes = Get-BenchmarkTimesForVersion -Version $version -Os "ubuntu-latest" -ArtifactsPath $ArtifactsDir
        Write-Host "  Ubuntu - Insert: $($ubuntuTimes.Insert)ms, Update: $($ubuntuTimes.Update)ms, Query: $($ubuntuTimes.Query)ms"
        
        # Update the content (Ubuntu is typically the primary platform)
        $pattern = "(\| $versionNumber\s+\|[^|]+\|[^|]+\|)\s*``TIME_PLACEHOLDER``\s*\|\s*``TIME_PLACEHOLDER``\s*\|\s*``TIME_PLACEHOLDER``"
        $replacement = "`$1 $($ubuntuTimes.Insert) | $($ubuntuTimes.Update) | $($ubuntuTimes.Query)"
        $content = $content -replace $pattern, $replacement
    }
    
    # Note: For simplicity, we're using Ubuntu times for both platforms in the table
    # If Windows-specific times are needed, they would need to be handled separately
}

# Save the updated content
Set-Content -Path $OutputFile -Value $content -NoNewline

Write-Host ""
Write-Host "DatabaseVersions.md updated successfully!"
Write-Host ""
Write-Host "Summary of changes:"
$remainingPlaceholders = ([regex]::Matches($content, "TIME_PLACEHOLDER")).Count
Write-Host "  Remaining TIME_PLACEHOLDER markers: $remainingPlaceholders"

if ($remainingPlaceholders -eq 0) {
    Write-Host "  ✓ All placeholders have been replaced!"
} else {
    Write-Host "  ⚠ Some placeholders remain - check for missing benchmark artifacts"
}
