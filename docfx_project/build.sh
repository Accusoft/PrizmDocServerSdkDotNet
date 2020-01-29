#!/usr/bin/env bash
set -e # stop on error
set +x # do not echo commands

docfx --version
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet restore ../PrizmDocServerSDK.sln
docfx metadata --warningsAsErrors
docfx build --warningsAsErrors
