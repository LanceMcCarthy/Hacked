#!/usr/bin/env bash
set -euo pipefail

APPLE_DEV_ID_APP_CERT_NAME="${APPLE_DEV_ID_APP_CERT_NAME:-}"
APPLE_DEV_ID_INSTALLER_CERT_NAME="${APPLE_DEV_ID_INSTALLER_CERT_NAME:-}"

security find-identity -v -p codesigning
security find-certificate -a -c "$APPLE_DEV_ID_INSTALLER_CERT_NAME" || true

if ! security find-identity -v -p codesigning | grep -F "$APPLE_DEV_ID_APP_CERT_NAME" >/dev/null; then
  echo "Missing Developer ID Application identity: $APPLE_DEV_ID_APP_CERT_NAME"
  exit 1
fi

if ! security find-certificate -a -c "$APPLE_DEV_ID_INSTALLER_CERT_NAME" >/dev/null; then
  echo "Missing Developer ID Installer certificate: $APPLE_DEV_ID_INSTALLER_CERT_NAME"
  exit 1
fi