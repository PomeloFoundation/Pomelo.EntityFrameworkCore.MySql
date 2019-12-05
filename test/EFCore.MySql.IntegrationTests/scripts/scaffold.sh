#!/bin/bash
cd $(dirname $0)
cd ../

set -e

rm -rf Scaffold
mkdir -p Scaffold

connection_string=$(dotnet run connectionString | tail -1)
./scripts/dotnet-ef.sh dbcontext scaffold "$connection_string" "Pomelo.EntityFrameworkCore.MySql" --output-dir "Scaffold" --table "DataTypesSimple" --table "DataTypesVariable"

error=false
files=( "DataTypesSimple.cs" "DataTypesVariable.cs" )
for file in "${files[@]}"
do
    if [ ! -f "Scaffold/$file" ]
    then
        >&2 echo "Failed to scaffold file: $file"
        error=true
    fi
done
if [ "$error" = true ] ; then
    exit 1
fi
