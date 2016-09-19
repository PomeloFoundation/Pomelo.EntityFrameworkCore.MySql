#!/usr/bin/env bash
cd $(dirname $0)
cd vegeta

RATE=${1:-50}
DURATION=${2:-5s}
TARGETS=${3:-async}
TARGETS=targets-$TARGETS.txt

echo "Rate = $RATE RPS"
echo "Duration = $DURATION"
echo "Targets = $TARGETS"

# warm up the JIT Compiler
vegeta attack -targets=$TARGETS -rate=10 -duration=1s > /dev/null 2>&1
# run the actual test
vegeta attack -targets=$TARGETS -rate="$RATE" -duration="$DURATION" | vegeta report