#!/bin/bash
cd $(dirname $0)
cd ../

set -e

rm -rf Scaffold
mkdir -p Scaffold

connection_string=$(dotnet run connectionString)
dotnet ef dbcontext scaffold -o "Scaffold" -t "DataTypesSimple" -t "DataTypesVariable" "$connection_string" "Pomelo.EntityFrameworkCore.MySql"

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
