#!/usr/bin/env bash
# Programmatic validation of packed .nupkg contents.
#
# Opens each package as a ZIP and asserts the expected layout, dependency
# groups, native assets, and the absence of build-machine paths or simulator
# slices where they don't belong. Runs on macOS and Linux (python3 + bash).
#
# Usage: eng/validate-packages.sh --version <pkg-version> --feed <dir-with-nupkgs> [--ios-only]
set -euo pipefail

VERSION=""
FEED=""
IOS_ONLY="false"
while [[ $# -gt 0 ]]; do
  case "$1" in
    --version) VERSION="$2"; shift 2 ;;
    --feed)    FEED="$(cd "$2" && pwd)"; shift 2 ;;
    --ios-only) IOS_ONLY="true"; shift ;;
    *) echo "Unknown argument: $1" >&2; exit 2 ;;
  esac
done
[[ -n "$VERSION" && -n "$FEED" ]] || { echo "Usage: eng/validate-packages.sh --version <v> --feed <dir> [--ios-only]" >&2; exit 2; }

python3 - "$FEED" "$VERSION" "$IOS_ONLY" <<'PYEOF'
import re
import sys
import zipfile
from pathlib import Path
from xml.etree import ElementTree

feed = Path(sys.argv[1])
version = sys.argv[2]
ios_only = len(sys.argv) > 3 and sys.argv[3] == "true"
failures = []


def check(cond, message):
    status = "ok " if cond else "FAIL"
    print(f"  [{status}] {message}")
    if not cond:
        failures.append(message)


def load(pkg_id):
    path = feed / f"{pkg_id}.{version}.nupkg"
    if not path.exists():
        failures.append(f"{path.name} missing from feed")
        print(f"  [FAIL] {path.name} missing from feed")
        return None, []
    z = zipfile.ZipFile(path)
    return z, z.namelist()


def nuspec(z, pkg_id):
    ns = {"n": ""}
    data = z.read(f"{pkg_id}.nuspec").decode("utf-8-sig")
    # Strip default namespace for simpler XPath.
    data = re.sub(r'xmlns="[^"]+"', "", data, count=1)
    return ElementTree.fromstring(data), data


def dependency_groups(root):
    groups = {}
    for g in root.findall(".//dependencies/group"):
        tfm = g.get("targetFramework") or ""
        groups[tfm] = {d.get("id"): d.get("version") for d in g.findall("dependency")}
    return groups


BAD_PATH_RX = re.compile(r"(/Users/[a-zA-Z0-9_]+/|C:\\Users\\|/home/runner|/private/tmp|obj/(Debug|Release)|(^|/)bin/(Debug|Release))", re.IGNORECASE)

# ── iOS binding package ─────────────────────────────────────────────────────
print(f"\n== Plugin.Maui.Intercom.iOS.Binding {version} ==")
z, names = load("Plugin.Maui.Intercom.iOS.Binding")
if z:
    root, raw = nuspec(z, "Plugin.Maui.Intercom.iOS.Binding")
    check(root.find(".//id").text == "Plugin.Maui.Intercom.iOS.Binding", "package id")
    check(root.find(".//version").text == version, f"package version == {version}")

    libs = [n for n in names if n.startswith("lib/net10.0-ios") and n.endswith(".dll")]
    check(len(libs) >= 1, f"managed dll(s) under lib/net10.0-ios*: {libs}")

    # Native assets ship via the classic iOS binding-resources sidecar:
    # lib/<tfm>/<Assembly>.resources.zip containing the NativeReference xcframework.
    # The .NET iOS SDK unpacks it in consuming apps and applies the NativeReference,
    # which is the supported transitive mechanism for IsBindingProject packages.
    import io
    res_names = [n for n in names if n.endswith(".resources.zip")]
    check(len(res_names) == 1, f"exactly one binding resources.zip ({res_names})")
    if res_names:
        rz = zipfile.ZipFile(io.BytesIO(z.read(res_names[0])))
        rn = rz.namelist()
        check("manifest" in rn, "resources.zip carries the binding manifest")
        check("Intercom.xcframework/ios-arm64/Intercom.framework/Intercom" in rn,
              "device Intercom.framework binary in resources.zip")
        check(any(n.startswith("Intercom.xcframework/ios-arm64_x86_64-simulator/Intercom.framework/Intercom")
                  for n in rn),
              "simulator Intercom.framework binary in resources.zip")
        check("Intercom.xcframework/ios-arm64/Intercom.framework/PrivacyInfo.xcprivacy" in rn,
              "PrivacyInfo.xcprivacy shipped with the device framework")
        for bundle in ("Intercom.bundle", "IntercomTranslations.bundle"):
            check(any(f"/ios-arm64/Intercom.framework/{bundle}/" in n for n in rn),
                  f"{bundle} resources present in device framework")
        roots = {n.split("/")[0] for n in rn}
        check(roots <= {"Intercom.xcframework", "manifest"},
              f"resources.zip contains only the Intercom.xcframework (roots: {sorted(roots)})")

    # Absolute/build-machine paths in MSBuild assets.
    for n in names:
        if n.endswith((".targets", ".props")):
            content = z.read(n).decode("utf-8", errors="replace")
            check(not BAD_PATH_RX.search(content), f"no machine-specific paths in {n}")

    check(not any(BAD_PATH_RX.search(n) for n in names), "no machine-specific package entry paths")

    groups = dependency_groups(root)
    print(f"  dependency groups: { {k: sorted(v) for k, v in groups.items()} }")

# ── Android binding package ────────────────────────────────────────────────
z, names = (None, [])
if ios_only:
    print("\n== Plugin.Maui.Intercom.Android.Binding: SKIPPED (--ios-only) ==")
else:
    print(f"\n== Plugin.Maui.Intercom.Android.Binding {version} ==")
    z, names = load("Plugin.Maui.Intercom.Android.Binding")
if z:
    root, _ = nuspec(z, "Plugin.Maui.Intercom.Android.Binding")
    check(root.find(".//version").text == version, f"package version == {version}")
    check(any(n.startswith("lib/net10.0-android") and n.endswith(".dll") for n in names),
          "managed dll under lib/net10.0-android*")
    check(any(n.endswith(".aar") for n in names), "bundled .aar present")

# ── Main package ────────────────────────────────────────────────────────────
print(f"\n== Plugin.Maui.Intercom {version} ==")
z, names = load("Plugin.Maui.Intercom")
if z:
    root, _ = nuspec(z, "Plugin.Maui.Intercom")
    check(root.find(".//version").text == version, f"package version == {version}")
    check(any(n.startswith("lib/net10.0-ios") and n.endswith("Plugin.Maui.Intercom.dll") for n in names),
          "iOS lib present")
    if not ios_only:
        check(any(n.startswith("lib/net10.0-android") and n.endswith("Plugin.Maui.Intercom.dll") for n in names),
              "Android lib present")

    groups = dependency_groups(root)
    ios_groups = {tfm: deps for tfm, deps in groups.items() if "ios" in tfm.lower()}
    android_groups = {tfm: deps for tfm, deps in groups.items() if "android" in tfm.lower()}
    check(bool(ios_groups), f"iOS dependency group exists ({list(groups)})")
    if not ios_only:
        check(bool(android_groups), f"Android dependency group exists ({list(groups)})")
    for tfm, deps in ios_groups.items():
        check(deps.get("Plugin.Maui.Intercom.iOS.Binding") == version,
              f"{tfm} depends on iOS binding {version} (got {deps.get('Plugin.Maui.Intercom.iOS.Binding')})")
        check("Plugin.Maui.Intercom.Android.Binding" not in deps,
              f"{tfm} does not leak the Android binding")
    for tfm, deps in android_groups.items():
        check(deps.get("Plugin.Maui.Intercom.Android.Binding") == version,
              f"{tfm} depends on Android binding {version}")
        check("Plugin.Maui.Intercom.iOS.Binding" not in deps,
              f"{tfm} does not leak the iOS binding")

    snupkg = feed / f"Plugin.Maui.Intercom.{version}.snupkg"
    check(snupkg.exists(), "symbols package (.snupkg) present for main package")

print()
if failures:
    print(f"Package validation FAILED ({len(failures)} problem(s)):")
    for f in failures:
        print(f"  - {f}")
    sys.exit(1)
print("Package validation PASSED.")
PYEOF
