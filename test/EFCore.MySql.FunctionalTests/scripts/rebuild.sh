#!/usr/bin/env bash
cd $(dirname $0)
cd ../

set -e
rm -f Migrations/*.cs
dotnet ef database drop -f
dotnet ef migrations add initial

set +e
# add using System.Collections.Generic to the migration files
find ./Migrations -name "*.cs" -type f | while read -r file; do
    grep -q "using System.Collections.Generic;" $file
    if [ $? -ne 0 ]; then
        sed -i '1s/^/using System.Collections.Generic;\n/' $file
    fi
done

set -e
dotnet ef database update
