#!/usr/bin/env bash
set -euo pipefail

INSTALLER_KC="$HOME/Library/Keychains/installer_signing_temp.keychain-db"
APP_KC="$HOME/Library/Keychains/application_signing_temp.keychain-db"

security list-keychains -d user -s "$INSTALLER_KC" "$APP_KC" login.keychain
echo "Keychain search list:"
security list-keychains -d user