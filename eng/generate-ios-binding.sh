#!/usr/bin/env bash
# Reproducible build + pack of the swift-dotnet-bindings based iOS binding.
#
# The binding is (re)generated from the vendored, pinned Intercom.xcframework at
# build time by the SwiftBindings.Sdk MSBuild project SDK (version pinned in
# global.json msbuild-sdks). Nothing is downloaded from Intercom here; use
# eng/update-intercom.sh to change the pinned Intercom SDK version.
#
# Usage: eng/generate-ios-binding.sh [--version <package-version>] [--output <dir>]
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BINDING_PROJ="$REPO_ROOT/src/macios/Intercom.iOS.Binding/Intercom.iOS.Binding.csproj"
OUTPUT_DIR="$REPO_ROOT/artifacts/packages"
VERSION_ARG=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --version) VERSION_ARG="-p:Version=$2"; shift 2 ;;
    --output)  OUTPUT_DIR="$2"; shift 2 ;;
    *) echo "Unknown argument: $1" >&2; exit 2 ;;
  esac
done

# ── Environment guards ──────────────────────────────────────────────────────
if [[ "$(uname -s)" != "Darwin" ]]; then
  echo "ERROR: the iOS binding can only be generated on macOS (Xcode is required)." >&2
  exit 1
fi

if ! command -v xcodebuild >/dev/null; then
  echo "ERROR: xcodebuild not found. Install Xcode 26 or later." >&2
  exit 1
fi

XCODE_VERSION="$(xcodebuild -version | awk 'NR==1{print $2}')"
XCODE_MAJOR="${XCODE_VERSION%%.*}"
if (( XCODE_MAJOR < 26 )); then
  echo "ERROR: Xcode 26+ is required by swift-dotnet-bindings; found Xcode $XCODE_VERSION." >&2
  exit 1
fi

if ! command -v dotnet >/dev/null; then
  echo "ERROR: dotnet not found. Install the .NET 10 SDK." >&2
  exit 1
fi

DOTNET_VERSION="$(cd "$REPO_ROOT" && dotnet --version)"
if [[ "${DOTNET_VERSION%%.*}" != "10" ]]; then
  echo "ERROR: .NET 10 SDK is required (global.json); resolved '$DOTNET_VERSION'." >&2
  exit 1
fi

# ── Pinned versions (single source of truth) ────────────────────────────────
prop() { sed -n "s:.*<$1>\(.*\)</$1>.*:\1:p" "$REPO_ROOT/Directory.Build.props" | head -1; }
INTERCOM_VERSION="$(prop IntercomIosSdkVersion)"
SDK_VERSION="$(sed -n 's/.*"SwiftBindings.Sdk": *"\([^"]*\)".*/\1/p' "$REPO_ROOT/global.json")"

VENDORED_VERSION="$(sed -n 's/.*Intercom iOS SDK - \([0-9.]*\).*/\1/p' "$REPO_ROOT/src/macios/Intercom.iOS.Binding/Intercom.xcframework/VERSION" | head -1)"
if [[ "$VENDORED_VERSION" != "$INTERCOM_VERSION" ]]; then
  echo "ERROR: vendored xcframework is $VENDORED_VERSION but Directory.Build.props pins $INTERCOM_VERSION." >&2
  exit 1
fi

echo "── Toolchain ───────────────────────────────────────────"
echo "swift-dotnet-bindings (SwiftBindings.Sdk): $SDK_VERSION"
echo "Intercom iOS SDK:                          $INTERCOM_VERSION"
echo "Xcode:                                     $XCODE_VERSION ($(xcode-select -p))"
echo "Swift:                                     $(swift --version 2>/dev/null | head -1)"
echo ".NET SDK:                                  $DOTNET_VERSION"
echo "────────────────────────────────────────────────────────"

# ── Clean previous generated output, then build + pack ──────────────────────
rm -rf "$REPO_ROOT/src/macios/Intercom.iOS.Binding/obj" \
       "$REPO_ROOT/src/macios/Intercom.iOS.Binding/bin"
mkdir -p "$OUTPUT_DIR"

dotnet build "$BINDING_PROJ" -c Release -p:SwiftGeneratorVerbosity=2 ${VERSION_ARG:+"$VERSION_ARG"}
dotnet pack  "$BINDING_PROJ" -c Release --no-build --output "$OUTPUT_DIR" ${VERSION_ARG:+"$VERSION_ARG"}

echo ""
echo "Packed:"
ls -1 "$OUTPUT_DIR"/Plugin.Maui.Intercom.iOS.Binding.*.nupkg
