![](nuget.png)
# Plugin.Maui.Intercom

`Plugin.Maui.Intercom` provides the ability to add [Intercom](https://www.intercom.com/) to your .NET MAUI application.

- **Android**: a classic binding of the Intercom Android SDK plus a small native wrapper.
- **iOS**: a binding of the official Intercom iOS `Intercom.xcframework`, generated with [swift-dotnet-bindings](https://github.com/justinwojo/swift-dotnet-bindings).

## Status

Both Android and iOS platforms are working.

<img width="403" height="696" alt="Screenshot 2026-01-20 134953" src="https://github.com/user-attachments/assets/9696d97e-87a2-450a-bd76-ed261101f2f0" />
<img width="395" height="505" alt="Screenshot 2026-01-20 124137" src="https://github.com/user-attachments/assets/c4f5a049-cdbe-46fc-bce4-bc1b4260c8d2" />

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.Intercom.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.Intercom/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.Intercom).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.Intercom`, or through the NuGet Package Manager in Visual Studio.

The platform binding packages (`Plugin.Maui.Intercom.iOS.Binding`, `Plugin.Maui.Intercom.Android.Binding`) are declared as platform-specific NuGet dependencies of the main package and restore automatically — never reference them directly.

### Supported Platforms and Versions

| | Version |
|----------|---------------------------|
| .NET | **.NET 10** (`net10.0-ios`, `net10.0-android`) |
| .NET MAUI | 10.x |
| iOS | 15.0+ |
| Android | 6.0 (API 23)+ |

### Native SDK Versions (pinned)

| Platform | Intercom SDK Version |
|----------|---------------------|
| Android  | 17.4.1              |
| iOS      | 18.7.2              |

> **Breaking change (0.7.0):** the plugin now targets .NET 10 only. .NET 9 (`net9.0-*`) consumers must stay on 0.6.x or earlier. The iOS binding was replaced: the former `MauiIntercomMaciOS` wrapper types and the public `DictionaryExtensions.ToNSDictionary` iOS helper were removed. The `IIntercom` interface itself is source-compatible, with three additions: `LogEvent(string name)`, `EnableLogging()` and `IsUserLoggedIn` — all implemented on both platforms.

## Setup

### MauiProgram.cs

Register Intercom in your `MauiProgram.cs`:

```csharp
using Plugin.Maui.Intercom;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseIntercom()  // Add this line
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}
```

### Android Configuration

Add the following permissions to your `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.INTERNET" />
```

### iOS Configuration

No additional configuration is required for iOS. The native `Intercom.framework` (including its resource bundles and `PrivacyInfo.xcprivacy`) is embedded and signed automatically by the binding package's MSBuild targets.

## API Usage

### Initialization

Initialize Intercom early in your app's lifecycle (e.g., in `App.xaml.cs` or your main page):

```csharp
using Plugin.Maui.Intercom;

// Initialize with your Intercom credentials
Intercom.Default.Initialize("your-api-key", "your-app-id");
```

You can find your API key and App ID in your [Intercom settings](https://app.intercom.com/a/apps/_/settings/android).

### User Registration

#### Register an unidentified user

```csharp
Intercom.Default.Register(
    onSuccess: () => Console.WriteLine("User registered successfully"),
    onFailure: (error) => Console.WriteLine($"Registration failed: {error}")
);
```

#### Register with User ID

```csharp
Intercom.Default.RegisterWithUserId(
    "user-123",
    onSuccess: () => Console.WriteLine("User registered"),
    onFailure: (error) => Console.WriteLine($"Failed: {error}")
);
```

#### Register with Email

```csharp
Intercom.Default.RegisterWithEmail(
    "user@example.com",
    onSuccess: () => Console.WriteLine("User registered"),
    onFailure: (error) => Console.WriteLine($"Failed: {error}")
);
```

### Identity Verification (Recommended)

For enhanced security, use [Identity Verification](https://developers.intercom.com/docs/build-an-integration/learn-more/security/identity-verification/identity-verification-web-ios/):

```csharp
// Generate the user hash on your server using HMAC-SHA256
// with your Intercom secret key and the user's identifier
Intercom.Default.SetUserHash("hmac-sha256-hash");
```

### Presenting Intercom UI

```csharp
Intercom.Default.PresentMessenger(null);            // Messenger
Intercom.Default.PresentMessenger("I need help");   // Messenger with a pre-filled composer
Intercom.Default.PresentHelpCenter();               // Help center space
Intercom.Default.PresentSupportCenter();            // Home space
Intercom.Default.PresentCarousel("carousel-id");    // A specific carousel
```

All presentation happens on the main thread automatically.

Intercom reports *every* Messenger failure the same way — a generic "something went wrong"
screen — so check that a user is actually registered before presenting, and turn on the native
SDK's own logging while diagnosing:

```csharp
Intercom.Default.EnableLogging();       // call before Initialize; not for release builds
Intercom.Default.Initialize(apiKey, appId);

if (!Intercom.Default.IsUserLoggedIn)
{
    Intercom.Default.Register();        // or RegisterWithEmail / RegisterWithUserId
}

Intercom.Default.PresentMessenger(null);
```

`EnableLogging` writes to the Xcode console on iOS and to logcat (tag `intercom`) on Android.
The usual causes of the error screen are: no logged-in user, an API key that belongs to the
other platform, an App ID that does not match the key, or identity verification enabled on the
workspace without a matching `SetUserHash` call *before* registration.

### Events

```csharp
Intercom.Default.LogEvent("clicked_checkout");
```

### Customization

```csharp
Intercom.Default.SetVisible(true);       // Show/hide the launcher
Intercom.Default.SetBottomPadding(100);  // Padding from the bottom of the screen
```

### Logout

```csharp
Intercom.Default.Logout();
```

### Dependency Injection

`UseIntercom()` registers `IIntercom` as a singleton, so you can constructor-inject it:

```csharp
public class MyViewModel
{
    private readonly IIntercom _intercom;

    public MyViewModel(IIntercom intercom) => _intercom = intercom;

    public void ShowMessenger() => _intercom.PresentMessenger(null);
}
```

## Architecture

Three NuGet packages, all versioned identically:

| Package | Role |
|---------|------|
| `Plugin.Maui.Intercom` | User-facing MAUI abstraction (`IIntercom`, DI setup) |
| `Plugin.Maui.Intercom.iOS.Binding` | iOS native binding, generated by swift-dotnet-bindings |
| `Plugin.Maui.Intercom.Android.Binding` | Android native binding (Gradle interop) |

The main package declares the binding packages as platform-conditional dependencies (`net10.0-ios` → iOS binding, `net10.0-android` → Android binding), so consumers only ever add `Plugin.Maui.Intercom`.

### How the iOS binding works

- The exact Intercom `Intercom.xcframework` (18.7.2) is **checked into the repository** at `src/macios/Intercom.iOS.Binding/` — ordinary builds never download anything. The SHA-256 of the official release archive is recorded in `eng/intercom-ios.sha256`.
- `src/macios/Intercom.iOS.Binding` uses the `SwiftBindings.Sdk` MSBuild project SDK (version pinned in `global.json` under `msbuild-sdks`). Intercom is a mixed Swift/Objective-C framework whose complete public API is exported through its ObjC umbrella header, so the binding uses the generator's pure-ObjC pipeline (`SwiftFrameworkType=ObjC` + `IsBindingProject=true`): at build time on macOS it parses the framework headers with clang, generates the bgen `ApiDefinition`, and compiles a single binding assembly (namespace `IntercomBinding`).
- The whole surface is generated — there is no hand-written supplement. Up to SwiftBindings.Sdk 0.17.0 the generator's `-fmodules` clang retry made clang build Intercom as a module, which collapsed each `#import <Intercom/SiblingHeader.h>` into a module import and silently dropped every declaration behind it (`ICMUserAttributes`, `IntercomContent`, the help-center types, the `Space`/`ContentType` NS_ENUMs). Two supplement files covered that gap; [SwiftBindings 0.18.0](https://github.com/justinwojo/swift-dotnet-bindings/releases/tag/sdk-v0.18.0) passes `-fmodule-name` on the retry so those headers parse textually, and the supplements were removed.
- The package uses the standard iOS binding layout: the managed assembly in `lib/net10.0-iosX.Y/` plus `Intercom.iOS.Binding.resources.zip` beside it containing the full `Intercom.xcframework` (device + simulator slices, resource bundles, `PrivacyInfo.xcprivacy`). The .NET iOS SDK unpacks it in consuming apps and applies the `NativeReference` automatically — embedding, linking and signing included.
- The remaining generated C# is a **build output, not committed**: the generator ships as a pinned NuGet SDK, so builds are deterministic from pinned inputs (SDK version + vendored xcframework) without every contributor installing anything. CI uploads the generated sources as an artifact for review.

## Building from Source

Requirements:

| Tool | Version |
|------|---------|
| .NET SDK | 10.0.1xx (`global.json`) |
| .NET MAUI workloads | `maui-ios`, `maui-android` |
| Xcode (iOS binding + apps) | 26+ (CI pins 26.6) |
| macOS | Required for anything iOS-native |
| JDK (Android binding) | 17 |

```bash
# Android binding (any OS with JDK 17)
dotnet pack src/android/Intercom.Android.Binding/Intercom.Android.Binding.csproj -c Release -o artifacts/packages

# iOS binding (macOS only — generates the binding and packs it)
eng/generate-ios-binding.sh --output artifacts/packages

# Main package, resolving the freshly packed bindings from the local folder
dotnet restore src/Plugin.Maui.Intercom/Plugin.Maui.Intercom.csproj --configfile <nuget.config pointing at artifacts/packages>
dotnet pack src/Plugin.Maui.Intercom/Plugin.Maui.Intercom.csproj -c Release --output artifacts/packages
```

> The iOS binding project is intentionally not in the solution file; it can only build on macOS.

### Clean-room package test

To prove the packages work from outside the source tree (this is what CI gates releases on):

```bash
eng/test-consumer.sh --version <version> --feed artifacts/packages --clear-cache
```

This creates a minimal MAUI app in `/tmp/icom-test` that references only `Plugin.Maui.Intercom` by package ID from a temporary NuGet feed, verifies the iOS binding resolves transitively, builds for the iOS simulator and (unsigned) for `ios-arm64`, and inspects the resulting `.app` for the embedded framework, resource bundles and privacy manifest.

```bash
eng/validate-packages.sh --version <version> --feed artifacts/packages
```

opens each `.nupkg` and asserts IDs, versions, dependency groups, native assets and the absence of machine-specific paths or simulator slices in device trees.

### Upgrading the Intercom iOS SDK

```bash
eng/update-intercom.sh <new-version> [<expected-sha256>]
eng/generate-ios-binding.sh
```

The update script downloads the exact tagged release from `intercom/intercom-ios`, verifies/records its SHA-256, replaces the vendored xcframework and updates the `IntercomIosSdkVersion` pin in `Directory.Build.props`. After regenerating, fix any compile errors in `src/Plugin.Maui.Intercom/Intercom.macios.cs` caused by upstream API changes.

To update the swift-dotnet-bindings generator, change the `SwiftBindings.Sdk` version in `global.json` (`msbuild-sdks`) — releases are tagged `sdk-vX.Y.Z` upstream.

## Troubleshooting

### Android: Compose Version Mismatch

If you encounter runtime crashes related to `NoSuchMethodError` in Compose classes, ensure you're using Intercom SDK 17.4.1 or later, which is compatible with AndroidX Compose BOM 2025.11.01.

### iOS: Build on Windows

iOS builds require macOS. The sample project skips iOS targets on Windows; the iOS binding project only builds on macOS with Xcode 26+.

### iOS: Missing Swift symbols / linker errors

The binding package ships the required wrapper frameworks and the `buildTransitive` targets add the `NativeReference`s automatically. If the linker reports missing `Intercom` or Swift symbols:
- confirm `Plugin.Maui.Intercom.iOS.Binding` appears in your app's resolved packages (`obj/project.assets.json`),
- clear the NuGet cache (`dotnet nuget locals all --clear`) and restore again,
- make sure you are on the .NET 10 iOS workload matching your Xcode version.

### iOS: Framework embedding / signing

`Intercom.framework` is dynamic: it must end up in `YourApp.app/Frameworks` and be code-signed with the app. Both are handled by the standard `NativeReference` pipeline; if a device build fails signing on the nested framework, verify your signing identity also applies to embedded frameworks (automatic signing does).

### iOS: Trimming / AOT

Release device builds use the default MAUI trimmer settings. The binding assemblies carry the metadata the trimmer needs; do not disable trimming globally. If you enable aggressive trimming (`TrimMode=full`) and hit a missing-selector issue at runtime, file an issue with the trimmed member name.

### iOS: Simulator

The generated binding may route a small number of members through Swift calling conventions that are limited under the Mono JIT on the iOS **simulator** (device builds are unaffected). The plugin's API surface uses Intercom's Objective-C entry points and is not affected.

## Acknowledgements

This project could not have come to be without these projects and people, thank you!

- [swift-dotnet-bindings](https://github.com/justinwojo/swift-dotnet-bindings) - The Swift/ObjC → .NET binding generator used for the iOS binding
- [.NET MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui) - For the original Native Library Interop pattern
- [Intercom](https://www.intercom.com/) - For their excellent customer messaging platform
