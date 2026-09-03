# Pomelo.EntityFrameworkCore.MySql

SOLUTION        := Pomelo.EFCore.MySql.sln
CONFIGURATION   ?= Debug
PACKAGE_OUTPUT  := artifacts/packages
SRC_PROJECTS    := \
	src/EFCore.MySql/EFCore.MySql.csproj \
	src/EFCore.MySql.NTS/EFCore.MySql.NTS.csproj \
	src/EFCore.MySql.Json.Microsoft/EFCore.MySql.Json.Microsoft.csproj \
	src/EFCore.MySql.Json.Newtonsoft/EFCore.MySql.Json.Newtonsoft.csproj

.DEFAULT_GOAL := help

.PHONY: help restore build rebuild test functional-tests package clean

help: ## List the available tasks (default)
	@echo "Pomelo.EntityFrameworkCore.MySql -- available make tasks:"
	@echo ""
	@grep -hE '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-16s\033[0m %s\n", $$1, $$2}'
	@echo ""
	@echo "Override the build configuration with CONFIGURATION=Release (default: Debug)."

restore: ## Restore NuGet dependencies for the solution
	dotnet restore $(SOLUTION)

build: ## Build the solution (CONFIGURATION=Debug|Release)
	dotnet build $(SOLUTION) -c $(CONFIGURATION)

rebuild: clean build ## Clean, then build the solution

test: ## Run the unit tests (no database required)
	dotnet test test/EFCore.MySql.Tests/EFCore.MySql.Tests.csproj -c $(CONFIGURATION)

functional-tests: ## Run the functional tests (requires a configured config.json + database)
	dotnet test test/EFCore.MySql.FunctionalTests/EFCore.MySql.FunctionalTests.csproj -c $(CONFIGURATION)

package: ## Create the release NuGet packages in artifacts/packages
	rm -rf $(PACKAGE_OUTPUT)
	$(foreach proj,$(SRC_PROJECTS),dotnet pack $(proj) -c Release -o $(PACKAGE_OUTPUT) &&) true
	@echo ""
	@echo "Release packages created in $(PACKAGE_OUTPUT):"
	@ls -1 $(PACKAGE_OUTPUT)/*.nupkg

clean: ## Remove build outputs and generated packages
	dotnet clean $(SOLUTION) -c $(CONFIGURATION) || true
	rm -rf $(PACKAGE_OUTPUT)
