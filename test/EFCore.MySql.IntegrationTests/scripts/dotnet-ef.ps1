#!/usr/bin/env powershell

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) "../")

dotnet ef `
    --msbuildprojectextensionspath "../../artifacts/obj/EFCore.MySql.IntegrationTests/" `
    @Args

Pop-Location
