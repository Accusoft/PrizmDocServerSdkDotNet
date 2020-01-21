#!/usr/bin/env bash
set -e # stop on error
set +x # do not echo commands

npx superstatic _site
