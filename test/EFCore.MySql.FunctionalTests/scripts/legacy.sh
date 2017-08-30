#!/bin/bash
cd $(dirname $0)
cd ../

set -e

for m in $(ls LegacyMigrations)
do
    rm -f Migrations/*.cs
    dotnet ef database drop -f
    cp LegacyMigrations/"$m"/*.csbak Migrations
    for f in $(ls Migrations/*.csbak)
    do
        mv $f $(printf "$f" | awk '{print substr($0, 1, length($0)-3)}')
    done
    dotnet ef migrations add current
    dotnet ef database update
    dotnet test
done
