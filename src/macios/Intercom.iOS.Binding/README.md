# Plugin.Maui.Intercom.iOS.Binding

iOS native bindings for the [Intercom iOS SDK](https://github.com/intercom/intercom-ios), generated with [swift-dotnet-bindings](https://github.com/justinwojo/swift-dotnet-bindings).

This package is a platform dependency of [Plugin.Maui.Intercom](https://www.nuget.org/packages/Plugin.Maui.Intercom) and is restored automatically when you reference the main package from a `net10.0-ios` MAUI application. You normally do not reference it directly.

## Contents

- Managed binding assemblies for the Intercom Swift/Objective-C API surface.
- The pinned Intercom `Intercom.xcframework` (device + simulator slices), including resource bundles and `PrivacyInfo.xcprivacy`.
- A generated native Swift wrapper xcframework.
- MSBuild `buildTransitive` targets that embed and link the native frameworks into the consuming application automatically.

## Versions

- Intercom iOS SDK version: pinned in this repository (see `Directory.Build.props`, `IntercomIosSdkVersion`).
- Minimum iOS version: 15.0.
- Target framework: `net10.0-ios`.

## Regeneration

The binding is regenerated from the vendored xcframework at build time on macOS with Xcode 26+ and the .NET 10 SDK. See `eng/generate-ios-binding.sh` in the repository for the reproducible build and `eng/update-intercom.sh` for upgrading the pinned Intercom version.
