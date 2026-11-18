# Pomelo.EntityFrameworkCore.MySql

Pomelo.EntityFrameworkCore.MySql is the most popular Entity Framework Core provider for MySQL compatible databases. It supports EF Core 10.0 and uses MySqlConnector for high-performance database server communication. The repository contains multiple NuGet packages and a comprehensive test suite.

Always reference these instructions first and only fallback to search or bash commands when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
- Install .NET 10.0.100 SDK: `curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10.0.100`
- Add to PATH: `export PATH="$HOME/.dotnet:$PATH"`
- Install EF Core tools: `dotnet tool restore` (takes ~1-2 seconds)
- **CRITICAL**: Create `/tmp/nuget.config` with clean NuGet sources due to network access issues:
  ```xml
  <?xml version="1.0" encoding="utf-8"?>
  <configuration>
    <packageSources>
      <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
      <add key="dotnet9" value="https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet9/nuget/v3/index.json" />
    </packageSources>
    <config>
      <add key="disableNugetaudit" value="true" />
    </config>
  </configuration>
  ```

### Build Commands (All Validated and Timed)
- **Package restore**: `dotnet restore --configfile /tmp/nuget.config --disable-parallel --no-cache --property WarningsAsErrors= --property TreatWarningsAsErrors=false` (takes ~2 seconds)
- **Debug build**: `dotnet build -c Debug --no-restore` (takes ~4 seconds). NEVER CANCEL: Set timeout to 10+ minutes.
- **Release build**: `dotnet build -c Release --no-restore` (takes ~11 seconds). NEVER CANCEL: Set timeout to 15+ minutes.
- **Setup test configs**: Required before building - copy example config files:
  ```bash
  cp test/EFCore.MySql.FunctionalTests/config.json.example test/EFCore.MySql.FunctionalTests/config.json
  cp test/EFCore.MySql.IntegrationTests/config.json.example test/EFCore.MySql.IntegrationTests/config.json
  ```

### Test Suite Overview
The repository has multiple test projects with different database requirements:

- **EFCore.MySql.Tests** (77 tests): True unit tests, NO database required. Takes ~3 seconds.
- **EFCore.MySql.FunctionalTests**: Database integration tests. REQUIRES MySQL database.
- **EFCore.MySql.IntegrationTests**: Full application integration tests with ASP.NET Core. REQUIRES MySQL database and complex setup.

### Database Setup for Tests
**CRITICAL**: Most tests require MySQL database setup:
```bash
# Start MySQL container (matches CI setup)
docker run --name mysql_test -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mysql:8.0.40

# Wait for MySQL to be ready (~30 seconds)
while ! docker exec mysql_test mysql --protocol=tcp -h localhost -P 3306 -u root -pPassword12! -e 'select 1;' &>/dev/null; do
  echo "Waiting for MySQL..."
  sleep 2
done
echo "MySQL is ready!"
```

### Running Tests (Validated Timings)
- **Unit tests only**: `dotnet test test/EFCore.MySql.Tests -c Debug --no-build --logger "console;verbosity=minimal"` (takes ~3 seconds, 77 tests)
- **Small functional test subset**: Takes ~8-9 seconds per test class
- **Full test suites**: NEVER CANCEL - Can take 20-45 minutes. Set timeout to 60+ minutes.

**WARNING**: The full test suite includes hundreds of tests across multiple projects. Each functional/integration test requires database connections and can be slow. Plan accordingly.

### EF Core Tools
- **Verify tools**: `dotnet ef --version` (should show version 10.0.0)
- **Scaffolding**: `dotnet ef dbcontext scaffold "<connection-string>" "Pomelo.EntityFrameworkCore.MySql" --output-dir <dir> --context <ContextName> --force`
- **Migration commands**: Available but require properly configured DbContext

### Integration Test Setup
The integration tests require additional setup:
```bash
# Integration tests have setup scripts (PowerShell)
./test/EFCore.MySql.IntegrationTests/scripts/rebuild.ps1
```
**NOTE**: Integration test setup takes ~45+ seconds and may have complex dependencies.

## Validation
- **Always run unit tests** after making changes: `dotnet test test/EFCore.MySql.Tests` (~3 seconds)
- **Test database connectivity** before running functional tests: Ensure MySQL container is running and accessible
- **Never run full test suite** unless specifically needed - it takes 20-45+ minutes
- **Build verification**: Always build both Debug and Release configurations to ensure compatibility

## Common Issues and Workarounds
- **Package restore failures**: Default NuGet.config references unreachable MyGet sources. Use the clean config provided above.
- **Package vulnerability warnings**: Treated as errors. Disable with `--property TreatWarningsAsErrors=false`
- **Test failures without database**: Functional and integration tests will fail if MySQL is not running
- **Network restrictions**: If external package sources are blocked, use `--ignore-failed-sources` with dotnet restore

## Repository Structure
Key projects:
- `src/EFCore.MySql/`: Main Entity Framework provider library
- `src/EFCore.MySql.Json.Microsoft/`: Microsoft System.Text.Json support
- `src/EFCore.MySql.Json.Newtonsoft/`: Newtonsoft.Json support  
- `src/EFCore.MySql.NTS/`: NetTopologySuite spatial support
- `test/EFCore.MySql.Tests/`: Unit tests (77 tests, ~3 seconds)
- `test/EFCore.MySql.FunctionalTests/`: Database integration tests
- `test/EFCore.MySql.IntegrationTests/`: Full ASP.NET Core integration tests

## Key Commands Reference (Copy-Paste Ready)
```bash
# Complete setup from fresh clone
export PATH="$HOME/.dotnet:$PATH"
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10
export PATH="$HOME/.dotnet:$PATH"

# Create clean NuGet config
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

# Install tools and restore packages
dotnet tool restore
dotnet restore --configfile /tmp/nuget.config --disable-parallel --no-cache --property WarningsAsErrors= --property TreatWarningsAsErrors=false

# Setup test configurations
cp test/EFCore.MySql.FunctionalTests/config.json.example test/EFCore.MySql.FunctionalTests/config.json
cp test/EFCore.MySql.IntegrationTests/config.json.example test/EFCore.MySql.IntegrationTests/config.json

# Build (NEVER CANCEL - set 15+ minute timeout)
dotnet build -c Debug --no-restore
dotnet build -c Release --no-restore

# Quick validation (unit tests only)
dotnet test test/EFCore.MySql.Tests -c Debug --no-build --logger "console;verbosity=minimal"

# For database tests, setup MySQL first:
docker run --name mysql_test -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mysql:8.0.40
```

Always use these exact commands for consistent results. Never skip the NuGet configuration step as it resolves critical network access issues.
