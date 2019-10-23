#!/usr/bin/env bash
set -e # stop on error
set +x # do not echo commands

dotnet restore ../PrizmDocServerSDK.sln
docfx metadata --warningsAsErrors
docfx build --warningsAsErrors
