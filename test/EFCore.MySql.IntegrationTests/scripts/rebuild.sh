#!/usr/bin/env bash
cd ..
rm -fR Migrations
dotnet tool restore;
dotnet ef database drop -f
dotnet ef migrations add initial
dotnet ef database update