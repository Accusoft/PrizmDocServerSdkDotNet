#!/usr/bin/env bash
set -e # stop on error
set +x # do not echo commands

# If GOOGLE_ANALYTICS_TRACKING_CODE is defined, alter the contents of head.tmpl.partial to use the actual value.
if [[ ! -z "${GOOGLE_ANALYTICS_TRACKING_CODE}" ]]; then
    # Make sure the file we are changing actually has the string we want to replace, "GOOGLE_ANALYTICS_TRACKING_CODE".
    # If it does not, fail immediately.
    # This ensures our build pipeline fails if we can't setup Google Analytics tracking correctly.
    grep 'GOOGLE_ANALYTICS_TRACKING_CODE' -q templates/accusoft/partials/head.tmpl.partial || (echo "head.templ.partial does not contain the token GOOGLE_ANALYTICS_TRACKINGCODE"; exit 1)

    # Perform the replacement.
    sed -i "s/GOOGLE_ANALYTICS_TRACKING_CODE/${GOOGLE_ANALYTICS_TRACKING_CODE}/g" templates/accusoft/partials/head.tmpl.partial
fi

docfx --version
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet restore ../PrizmDocServerSDK.sln
docfx metadata --warningsAsErrors
docfx build --warningsAsErrors
