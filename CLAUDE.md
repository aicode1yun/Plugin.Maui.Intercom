# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Plugin.Maui.Intercom is a .NET MAUI plugin that provides Intercom integration using Native Library Interop (NLI). It wraps the native Intercom SDK for Android and iOS/macOS platforms.

**Status**: Both Android and iOS platforms are working.

## Build Commands

```bash
# Build the main solution (includes binding projects and sample)
dotnet build src/Plugin.Maui.Intercom.sln -c Release

# Build Android-only
dotnet build src/Plugin.Maui.Intercom/Plugin.Maui.Intercom.csproj -c Release -f net9.0-android

# Build iOS-only (requires macOS)
dotnet build src/Plugin.Maui.Intercom/Plugin.Maui.Intercom.csproj -c Release -f net9.0-ios

# Run the sample app on Android
dotnet build src/sample/MauiSample.csproj -c Debug -f net9.0-android -t:Run

# Run the sample app on iOS (requires macOS)
dotnet build src/sample/MauiSample.csproj -c Debug -f net9.0-ios -t:Run
```

## Architecture

### Project Structure

- `src/Plugin.Maui.Intercom/` - Main MAUI plugin library (multi-targeted net9.0-android;net9.0-ios)
- `src/android/Intercom.Android.Binding/` - Android native binding project using Gradle interop
- `src/android/native/` - Native Android Kotlin/Java code (MauiIntercom module)
- `src/macios/Intercom.MaciOS.Binding/` - iOS/macOS native binding project
- `src/macios/native/NewBinding/` - Native Swift wrapper for iOS Intercom SDK
- `src/sample/` - Sample MAUI application demonstrating usage

### Native Library Interop (NLI) Pattern

This project uses the MAUI Native Library Interop pattern:

1. **Android**:
   - Native Java code in `src/android/native/mauiintercom/` wraps the Intercom Android SDK
   - `AndroidGradleProject` in the binding .csproj references `build.gradle.kts`
   - Binding automatically generates C# wrappers from Java code

2. **iOS**:
   - Native Swift code in `src/macios/native/NewBinding/` wraps the Intercom iOS SDK
   - `XcodeProject` in the binding .csproj references the Xcode project
   - Binding uses `ApiDefinition.cs` and `StructsAndEnums.cs` for Objective-C interop

### Platform-Specific Code Pattern

The plugin uses file suffixes for platform-specific implementations:
- `*.shared.cs` - Shared code (all platforms)
- `*.android.cs` - Android-specific implementation
- `*.macios.cs` - iOS/macOS-specific implementation
- `*.net.cs` - Generic .NET fallback

### Key Classes

- `IIntercom` - Public interface defining all Intercom operations
- `Intercom` - Static accessor class providing `Intercom.Default` singleton
- `IntercomImplementation` - Platform-specific implementations

## Dependencies

### Android Native Dependencies

The Android binding includes numerous AAR files in `src/android/Intercom.Android.Binding/Jars/`:
- Intercom SDK (17.4.1)
- Coil image loading library (2.7.0)
- Various AndroidX Compose dependencies

Note: The Intercom SDK version must be compatible with the Xamarin.AndroidX.Compose packages. Version 17.4.1 is compatible with Compose BOM 2025.11.01.

Many Xamarin.AndroidX packages are pinned to specific versions in the .csproj to avoid downgrade warnings.

### iOS Native Dependencies

The iOS binding uses the Intercom Swift SDK via Swift Package Manager in the Xcode project.

## Configuration Notes

- Sample app uses `appsettings.json` for configuration (including Intercom API keys)
- Intercom requires initialization with API key and App ID before use
- User registration can be done with email, userId, or as unidentified user
