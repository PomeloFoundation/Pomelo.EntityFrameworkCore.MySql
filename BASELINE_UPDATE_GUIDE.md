# Complete Guide: Updating EF Core 10 Scaffolding Baselines on Ubuntu

This guide provides step-by-step instructions for completing the EF Core 10 scaffolding baseline updates locally on Ubuntu.

## Prerequisites

### 1. Install .NET 10 SDK

```bash
# Download and install .NET 10 SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10.0.100

# Add to PATH (add to ~/.bashrc for persistence)
export PATH="$HOME/.dotnet:$PATH"

# Verify installation
dotnet --version
# Should output: 10.0.100
```

### 2. Install Docker (for MariaDB)

```bash
# Install Docker if not already installed
sudo apt-get update
sudo apt-get install -y docker.io
sudo systemctl start docker
sudo systemctl enable docker

# Add your user to docker group (requires logout/login)
sudo usermod -aG docker $USER
```

### 3. Clone the Repository

```bash
# Clone the repository
git clone https://github.com/microting/Microting.EntityFrameworkCore.MySql.git
cd Microting.EntityFrameworkCore.MySql

# Switch to the feature branch
git checkout copilot/update-baseline-files-ef-core-10
```

## Environment Setup

### 1. Create Clean NuGet Configuration

```bash
# Create a clean nuget.config to avoid package source issues
cat > /tmp/nuget.config << 'EOF'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <config>
    <add key="disableNugetaudit" value="true" />
  </config>
</configuration>
EOF
```

### 2. Restore Dependencies and Build

```bash
# Restore dotnet tools
dotnet tool restore

# Restore NuGet packages
dotnet restore --configfile /tmp/nuget.config --disable-parallel --no-cache \
    --property WarningsAsErrors= --property TreatWarningsAsErrors=false

# Copy test configuration files
cp test/EFCore.MySql.FunctionalTests/config.json.example test/EFCore.MySql.FunctionalTests/config.json
cp test/EFCore.MySql.IntegrationTests/config.json.example test/EFCore.MySql.IntegrationTests/config.json

# Build the project (takes ~20 seconds)
dotnet build -c Debug --no-restore
```

### 3. Start MariaDB 11.6.2

```bash
# Start MariaDB container
docker run --name mariadb_baseline \
    -e MYSQL_ROOT_PASSWORD=Password12! \
    -p 127.0.0.1:3306:3306 \
    -d mariadb:11.6.2

# Wait for MariaDB to be ready (takes ~30 seconds)
echo "Waiting for MariaDB to start..."
for i in {1..30}; do
    if docker exec mariadb_baseline mysql --protocol=tcp -h localhost -P 3306 \
        -u root -pPassword12! -e 'SELECT 1;' &>/dev/null; then
        echo "MariaDB is ready!"
        break
    fi
    echo "Attempt $i: Waiting..."
    sleep 2
done
```

## Method 1: Using EF Core Reference Baselines (Recommended)

This is the fastest and most reliable method.

### 1. Clone EF Core Repository

```bash
# Clone EF Core repository in a separate directory
cd /tmp
git clone --depth 1 --branch release/10.0 https://github.com/dotnet/efcore.git
cd efcore
```

### 2. Copy SQLite Baselines as Reference

```bash
# Navigate back to repo
cd ~/Microting.EntityFrameworkCore.MySql

# Create a script to copy and adapt baselines
cat > /tmp/copy_baselines.sh << 'EOFSCRIPT'
#!/bin/bash

EFCORE_BASELINES="/tmp/efcore/test/EFCore.Sqlite.FunctionalTests/Scaffolding/Baselines"
POMELO_BASELINES="test/EFCore.MySql.FunctionalTests/Scaffolding/Baselines"

# List of baseline directories to copy
DIRS=(
    "BigModel"
    "CheckConstraints"
    "ComplexTypes"
    "Custom_function_parameter_type_mapping"
    "Custom_function_type_mapping"
    "DbFunctions"
    "Dynamic_schema"
    "No_NativeAOT"
    "Sequences"
    "SimpleModel"
    "Tpc"
    "Tpc_Sprocs"
    "Triggers"
)

echo "Copying baseline files from EF Core Sqlite..."

for dir in "${DIRS[@]}"; do
    if [ -d "$EFCORE_BASELINES/$dir" ]; then
        echo "Processing $dir..."
        
        # Copy all EntityType files
        find "$EFCORE_BASELINES/$dir" -name "*EntityType.cs" -type f | while read file; do
            basename=$(basename "$file")
            dest="$POMELO_BASELINES/$dir/$basename"
            
            if [ -f "$dest" ]; then
                # Copy and adapt for MySQL
                cp "$file" "$dest"
                
                # Replace Sqlite-specific imports and types
                sed -i 's/using Microsoft.EntityFrameworkCore.Sqlite/using Microting.EntityFrameworkCore.MySql/g' "$dest"
                sed -i 's/SqliteTypeMapping/MySqlTypeMapping/g' "$dest"
                sed -i 's/SqliteStringTypeMapping/MySqlStringTypeMapping/g' "$dest"
                sed -i 's/SqliteIntTypeMapping/MySqlIntTypeMapping/g' "$dest"
                sed -i 's/SqliteDateTimeTypeMapping/MySqlDateTimeTypeMapping/g' "$dest"
                sed -i 's/SqliteBoolTypeMapping/MySqlBoolTypeMapping/g' "$dest"
                sed -i 's/SqliteByteArrayTypeMapping/MySqlByteArrayTypeMapping/g' "$dest"
                sed -i 's/SqliteGuidTypeMapping/MySqlGuidTypeMapping/g' "$dest"
                sed -i 's/SqliteValueGenerationStrategy/MySqlValueGenerationStrategy/g' "$dest"
                sed -i 's/Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal/Microting.EntityFrameworkCore.MySql.Storage.Internal/g' "$dest"
                
                # Add MySQL-specific import if needed
                if grep -q "MySqlTypeMapping" "$dest" && ! grep -q "using Microting.EntityFrameworkCore.MySql.Storage.Internal" "$dest"; then
                    sed -i '/using Microsoft.EntityFrameworkCore.Storage;/a using Microting.EntityFrameworkCore.MySql.Storage.Internal;' "$dest"
                fi
                
                echo "  ✓ Updated $basename"
            fi
        done
        
        # Copy other necessary files
        for file in "$EFCORE_BASELINES/$dir"/*.cs; do
            basename=$(basename "$file")
            dest="$POMELO_BASELINES/$dir/$basename"
            
            if [[ ! "$basename" =~ EntityType\.cs$ ]] && [ -f "$dest" ]; then
                cp "$file" "$dest"
                echo "  ✓ Updated $basename"
            fi
        done
    fi
done

echo ""
echo "Baseline files copied and adapted!"
EOFSCRIPT

chmod +x /tmp/copy_baselines.sh
/tmp/copy_baselines.sh
```

### 3. Verify and Test

```bash
# Run a single test to verify
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest.SimpleModel" \
    --logger "console;verbosity=normal"

# If successful, run all scaffolding tests
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest" \
    --logger "console;verbosity=normal"
```

## Method 2: Extract Generated Code from Test Framework

This method requires modifying the EF Core test framework temporarily.

### 1. Locate the Test Assembly

```bash
# Find where CompiledModelTestBase is loaded from
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest.SimpleModel" \
    --list-tests 2>&1 | grep "CompiledModel"
```

### 2. Create a Modified Test to Export Baselines

```bash
# Create a helper to capture generated code
cat > test/EFCore.MySql.FunctionalTests/Scaffolding/BaselineCaptureHelper.cs << 'EOF'
using System;
using System.IO;
using System.Linq;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Scaffolding
{
    public static class BaselineCaptureHelper
    {
        public static void CaptureBaseline(string testName, string generatedCode, string fileName)
        {
            var outputDir = Path.Combine(
                Path.GetDirectoryName(typeof(BaselineCaptureHelper).Assembly.Location),
                "GeneratedBaselines",
                testName);
            
            Directory.CreateDirectory(outputDir);
            
            var outputPath = Path.Combine(outputDir, fileName);
            File.WriteAllText(outputPath, generatedCode);
            
            Console.WriteLine($"[BASELINE CAPTURED] {outputPath}");
        }
    }
}
EOF
```

### 3. Run Tests with Capture

```bash
# Run tests - they will fail but generate output
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest" \
    --logger "console;verbosity=detailed" 2>&1 | tee /tmp/baseline_output.log

# Check for captured baselines (if helper was integrated)
find test/EFCore.MySql.FunctionalTests/bin -name "GeneratedBaselines" -type d
```

## Method 3: Manual Pattern-Based Updates

For remaining files that need manual updates:

### 1. Create Update Script

```bash
cat > /tmp/update_remaining_baselines.py << 'EOFPYTHON'
#!/usr/bin/env python3
import os
import re
from pathlib import Path

def update_setsetter(content):
    """Fix SetSetter and SetMaterializationSetter to return instance."""
    
    # Pattern: Find property.SetSetter((Type instance, ValueType value) => Assignment);
    # Replace with: property.SetSetter(Type (Type instance, ValueType value) => { Assignment; return instance; });
    
    # This regex captures the full setter assignment
    pattern = r'(\w+)\.Set(Materialization)?Setter\(\s*\(([^)]+instance[^)]+)\)\s*=>\s*([^;]+);'
    
    def replacement(match):
        prop_name = match.group(1)
        is_materialization = match.group(2) or ""
        params = match.group(3)
        assignment = match.group(4)
        
        # Extract type from parameters (first word before space)
        type_match = re.match(r'(\S+(?:<[^>]+>)?)\s+instance', params)
        if not type_match:
            return match.group(0)  # Return unchanged if can't parse
        
        entity_type = type_match.group(1)
        
        return f'''{prop_name}.Set{is_materialization}Setter(
                {entity_type} ({params}) =>
                {{
                    {assignment};
                    return instance;
                }});'''
    
    content = re.sub(pattern, replacement, content, flags=re.MULTILINE)
    return content

def update_file(filepath):
    """Update a single baseline file."""
    print(f"Processing: {filepath}")
    
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    original = content
    content = update_setsetter(content)
    
    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"  ✓ Updated")
        return True
    else:
        print(f"  - No changes")
        return False

def main():
    baselines_dir = Path("test/EFCore.MySql.FunctionalTests/Scaffolding/Baselines")
    files = list(baselines_dir.glob("**/*EntityType.cs"))
    
    print(f"Found {len(files)} files\n")
    
    updated = 0
    for filepath in sorted(files):
        if update_file(filepath):
            updated += 1
    
    print(f"\nUpdated {updated} of {len(files)} files")

if __name__ == "__main__":
    main()
EOFPYTHON

chmod +x /tmp/update_remaining_baselines.py
cd ~/Microting.EntityFrameworkCore.MySql
python3 /tmp/update_remaining_baselines.py
```

### 2. Test Individual Files

```bash
# Test after each major update
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest.SimpleModel"

dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest.BigModel"
```

## Validation and Commit

### 1. Run All Scaffolding Tests

```bash
# Run all compiled model tests (may take 5-10 minutes)
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest" \
    --logger "console;verbosity=normal" | tee /tmp/test_results.log

# Check for passing tests
grep -E "Passed|Failed" /tmp/test_results.log
```

### 2. Review Changes

```bash
# Check which files were modified
git status

# Review changes
git diff test/EFCore.MySql.FunctionalTests/Scaffolding/Baselines/

# Check specific file if needed
git diff test/EFCore.MySql.FunctionalTests/Scaffolding/Baselines/SimpleModel/DependentDerivedEntityType.cs
```

### 3. Commit Changes

```bash
# Stage all baseline changes
git add test/EFCore.MySql.FunctionalTests/Scaffolding/Baselines/

# Commit with descriptive message
git commit -m "Complete EF Core 10 compiled model baseline updates

- Updated all 86 EntityType baseline files to EF Core 10 format
- SetGetter: Reduced from 4 to 2 lambda parameters
- SetSetter/SetMaterializationSetter: Now return instance with block expressions
- SetAccessors: Reduced from 5 to 4 lambdas, uses IInternalEntry cast
- All scaffolding tests now passing"

# Push changes
git push origin copilot/update-baseline-files-ef-core-10
```

## Troubleshooting

### MariaDB Connection Issues

```bash
# Check if MariaDB is running
docker ps | grep mariadb

# View MariaDB logs
docker logs mariadb_baseline

# Restart MariaDB if needed
docker stop mariadb_baseline
docker rm mariadb_baseline
# Then rerun the start command from step 3
```

### Build Failures

```bash
# Clean and rebuild
dotnet clean
rm -rf */bin */obj
dotnet restore --configfile /tmp/nuget.config --force
dotnet build -c Debug --no-restore
```

### Test Failures

```bash
# Get detailed error information
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build \
    --filter "FullyQualifiedName~CompiledModelMySqlTest.SimpleModel" \
    --logger "console;verbosity=detailed" 2>&1 | less

# Check for specific error patterns
# Look for "Assert.Equal() Failure" to see exact differences
```

### Viewing Exact Differences

```bash
# If a test fails with position information, extract that section
# Example: If error says "pos 3062", view around that position
# This requires custom tooling or examining the generated code directly
```

## Summary

**Recommended Approach:** Method 1 (Using EF Core Reference Baselines)
- **Time Required:** 1-2 hours
- **Reliability:** Highest
- **Steps:** Clone EF Core repo → Copy Sqlite baselines → Adapt for MySQL → Test

**Alternative:** Method 2 (Extract from Test Framework)
- **Time Required:** 2-3 hours
- **Reliability:** High (requires framework modification)
- **Best for:** When you want to see exact generated output

**Fallback:** Method 3 (Manual Pattern Updates)
- **Time Required:** 4-6 hours
- **Reliability:** Medium (requires careful validation)
- **Best for:** Small number of remaining files after automated updates

## Clean Up

```bash
# Stop and remove MariaDB container
docker stop mariadb_baseline
docker rm mariadb_baseline

# Remove temporary EF Core clone
rm -rf /tmp/efcore

# Remove temporary scripts
rm -f /tmp/copy_baselines.sh /tmp/update_remaining_baselines.py /tmp/nuget.config
```

## Additional Resources

- [EF Core 10 Release Notes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew)
- [EF Core Breaking Changes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/breaking-changes)
- [Microting MySQL Provider Documentation](https://github.com/microting/Pomelo.EntityFrameworkCore.MySql)
