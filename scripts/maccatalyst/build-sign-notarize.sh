#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

PROJECT_PATH="${PROJECT_PATH:-src/Hacked.Maui/Hacked.Maui.csproj}"
TFM="${TFM:-net10.0-maccatalyst}"
CONFIGURATION="${CONFIGURATION:-Release}"
APP_VERSION="${APP_VERSION:-}"
ARTIFACTS_DIR="${ARTIFACTS_DIR:-artifacts/maccatalyst}"
APP_BUNDLE_PATH="${APP_BUNDLE_PATH:-src/Hacked.Maui/bin/Release/net10.0-maccatalyst/Hacked.app}"

CODESIGN_KEY="${CODESIGN_KEY:-Developer ID Application: Lancelot Software, LLC (L65255N3F7)}"
INSTALLER_SIGN_ID="${INSTALLER_SIGN_ID:-Developer ID Installer: Lancelot Software, LLC (L65255N3F7)}"

APPLE_NOTARY_APPLE_ID="${APPLE_NOTARY_APPLE_ID:-}"
APPLE_NOTARY_APP_PASSWORD="${APPLE_NOTARY_APP_PASSWORD:-}"
APPLE_NOTARY_TEAM_ID="${APPLE_NOTARY_TEAM_ID:-}"

SIGNED_ZIP_PATH="$ARTIFACTS_DIR/Hacked-MacCatalyst-Release-signed.zip"
NOTARIZED_ZIP_PATH="$ARTIFACTS_DIR/Hacked-MacCatalyst-Release-notarized.zip"
SIGNED_PKG_PATH="$ARTIFACTS_DIR/Hacked-MacCatalyst-Release-signed.pkg"

if [[ -z "$APPLE_NOTARY_APPLE_ID" || -z "$APPLE_NOTARY_APP_PASSWORD" || -z "$APPLE_NOTARY_TEAM_ID" ]]; then
  echo "Missing notarization credentials."
  echo "Set APPLE_NOTARY_APPLE_ID, APPLE_NOTARY_APP_PASSWORD, and APPLE_NOTARY_TEAM_ID."
  exit 1
fi

mkdir -p "$ARTIFACTS_DIR"

echo "Cleaning and publishing MacCatalyst app..."
dotnet clean "$PROJECT_PATH" -c "$CONFIGURATION" -f "$TFM"

VERSION_ARGS=""
if [[ -n "$APP_VERSION" ]]; then
  VERSION_ARGS="-p:ApplicationVersion=$APP_VERSION -p:ApplicationDisplayVersion=$APP_VERSION"
fi

dotnet publish "$PROJECT_PATH" -c "$CONFIGURATION" -f "$TFM" \
  -p:CodesignKey="$CODESIGN_KEY" \
  -p:UseHardenedRuntime=true \
  $VERSION_ARGS

if [[ ! -d "$APP_BUNDLE_PATH" ]]; then
  echo "Expected app bundle not found at $APP_BUNDLE_PATH"
  exit 1
fi

echo "Verifying app signature..."
codesign -dv --verbose=2 "$APP_BUNDLE_PATH" >/dev/null 2>&1

echo "Creating signed app zip and installer pkg..."
rm -f "$SIGNED_ZIP_PATH" "$NOTARIZED_ZIP_PATH" "$SIGNED_PKG_PATH"
ditto -c -k --sequesterRsrc --keepParent "$APP_BUNDLE_PATH" "$SIGNED_ZIP_PATH"
productbuild --component "$APP_BUNDLE_PATH" /Applications --sign "$INSTALLER_SIGN_ID" "$SIGNED_PKG_PATH"

echo "Submitting pkg for notarization..."
pkg_submit_json="$(xcrun notarytool submit "$SIGNED_PKG_PATH" \
  --apple-id "$APPLE_NOTARY_APPLE_ID" \
  --team-id "$APPLE_NOTARY_TEAM_ID" \
  --password "$APPLE_NOTARY_APP_PASSWORD" \
  --wait --output-format json)"
echo "$pkg_submit_json"
if ! grep -q '"status":"Accepted"' <<< "$pkg_submit_json"; then
  echo "Notarization failed for pkg."
  exit 1
fi

echo "Stapling and validating pkg..."
xcrun stapler staple "$SIGNED_PKG_PATH"
xcrun stapler validate "$SIGNED_PKG_PATH"

echo "Submitting signed app zip for notarization..."
zip_submit_json="$(xcrun notarytool submit "$SIGNED_ZIP_PATH" \
  --apple-id "$APPLE_NOTARY_APPLE_ID" \
  --team-id "$APPLE_NOTARY_TEAM_ID" \
  --password "$APPLE_NOTARY_APP_PASSWORD" \
  --wait --output-format json)"
echo "$zip_submit_json"
if ! grep -q '"status":"Accepted"' <<< "$zip_submit_json"; then
  echo "Notarization failed for app zip."
  exit 1
fi

echo "Stapling and validating app..."
xcrun stapler staple "$APP_BUNDLE_PATH"
xcrun stapler validate "$APP_BUNDLE_PATH"

# Recreate the final distributable zip from the stapled app.
ditto -c -k --sequesterRsrc --keepParent "$APP_BUNDLE_PATH" "$NOTARIZED_ZIP_PATH"

echo "Gatekeeper checks..."
spctl -a -t exec -vv "$APP_BUNDLE_PATH"
spctl -a -t install -vv "$SIGNED_PKG_PATH"

echo "Done. Artifacts:"
echo "- $SIGNED_PKG_PATH"
echo "- $NOTARIZED_ZIP_PATH"
