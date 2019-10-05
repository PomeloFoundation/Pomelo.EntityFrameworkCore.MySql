#!/bin/bash
cd $(dirname $0)
cd ../

set -e

for m in $(ls LegacyMigrations)
do
    rm -f Migrations/*.cs
    ./scripts/dotnet-ef.sh database drop -f
    cp LegacyMigrations/"$m"/*.csbak Migrations
    for f in $(ls Migrations/*.csbak)
    do
        mv $f $(printf "$f" | awk '{print substr($0, 1, length($0)-3)}')
    done
    ./scripts/dotnet-ef.sh migrations add current
    ./scripts/dotnet-ef.sh database update
    dotnet test
done
