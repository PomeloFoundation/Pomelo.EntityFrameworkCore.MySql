$ErrorActionPreference = "Stop"

$repoFolder = $PSScriptRoot
dotnet --info

##########################
# BEGIN REPOSITORY TESTS
##########################

# restore
cd "$repoFolder"
dotnet restore
if ($LASTEXITCODE -ne 0){
    exit $LASTEXITCODE;
}

# supress logging output (exceptions will still be printed)
cp -Force test\EFCore.MySql.FunctionalTests\appsettings.ci.json test\EFCore.MySql.FunctionalTests\appsettings.json

# build to verify no build errors
cd (Join-Path $repoFolder (Join-Path "src" "EFCore.MySql"))
dotnet build -c Release
if ($LASTEXITCODE -ne 0){
    exit $LASTEXITCODE;
}

# run unit tests
cd (Join-Path $repoFolder (Join-Path "test" "EFCore.MySql.Tests"))
dotnet test -c Release
if ($LASTEXITCODE -ne 0){
    exit $LASTEXITCODE;
}

# run functional tests if not on MyGet
if ($env:BuildRunner -ne "MyGet"){
    cd (Join-Path $repoFolder (Join-Path "test" "EFCore.MySql.FunctionalTests"))
    cp config.json.example config.json

    echo "Building Migrations"
    scripts\rebuild.ps1

    echo "Test applying migrations"
    dotnet run -c Release testMigrate
    if ($LASTEXITCODE -ne 0){
        exit $LASTEXITCODE;
    }

    echo "Test with EF_BATCH_SIZE=1"
    dotnet test -c Release
    if ($LASTEXITCODE -ne 0){
        exit $LASTEXITCODE;
    }
    
    echo "Test with EF_BATCH_SIZE=10"
    $env:EF_BATCH_SIZE = 10
    dotnet test -c Release
    if ($LASTEXITCODE -ne 0){
        exit $LASTEXITCODE;
    }
}

##########################
# END REPOSITORY TESTS
##########################

# MyGet expects nuget packages to be build
cd (Join-Path $repoFolder (Join-Path "src" "EFCore.MySql"))
if ($env:BuildRunner -eq "MyGet"){
    dotnet pack -c Release
    if ($LASTEXITCODE -ne 0){
        exit $LASTEXITCODE;
    }
}
