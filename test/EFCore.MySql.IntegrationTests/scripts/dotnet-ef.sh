#!/usr/bin/env bash

cd $(dirname $0)
cd ../

exec dotnet ef \
    --msbuildprojectextensionspath "../../artifacts/obj/EFCore.MySql.IntegrationTests/" \
    "$@"
