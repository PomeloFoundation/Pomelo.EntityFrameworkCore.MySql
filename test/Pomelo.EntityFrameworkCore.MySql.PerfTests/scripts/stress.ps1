#!/usr/bin/env powershell

Push-Location (Join-Path (Split-Path $MyInvocation.MyCommand.Path) vegeta)

$RATE="50"
$DURATION="5s"
$TARGETS="async"
if ($args.count -ge 1){ $RATE=$args[0] }
if ($args.count -ge 2){ $DURATION=$args[1] }
if ($args.count -ge 3){ $TARGETS=$args[2] }
$TARGETS="targets-"+$TARGETS+".txt"

Write-Host "Rate = $RATE RPS"
Write-Host "Duration = $DURATION"
Write-Host "Targets = $TARGETS"

# warm up the JIT Compiler
vegeta attack -targets="$TARGETS" -rate=10 -duration=1s | Out-Null
# run the actual test
# need to save to results.bin since piping will encode data
vegeta attack -targets="$TARGETS" -rate="$RATE" -duration="$DURATION" -output="result.bin"
vegeta report -inputs="result.bin"
rm "result.bin"

Pop-Location
