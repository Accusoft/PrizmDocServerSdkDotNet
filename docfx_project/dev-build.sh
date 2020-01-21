#!/usr/bin/env bash
set -e # stop on error
set +x # do not echo commands

docker run --rm -v $(pwd)/..:/source -w /source/docfx_project nexus.jpg.com:9443/accusoft/docfx ./build.sh
