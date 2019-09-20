#!/usr/bin/env bash

[ -z "${DOTNET_HOME:-}" ] && DOTNET_HOME="$HOME/.dotnet"
export PATH="$DOTNET_HOME:$PATH"

echo "DOTNET_HOME = $DOTNET_HOME"

exec "$@"
