#!/bin/bash
cd $(dirname $0)
cd ../

rm -rf Scaffold
mkdir -p Scaffold
dotnet ef dbcontext scaffold -o "Scaffold" -t "DataTypesSimple" -t "DataTypesVariable" "$1" "Pomelo.EntityFrameworkCore.MySql"

