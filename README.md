![](nuget.png)
# Plugin.Maui.Intercom

`Plugin.Maui.Intercom` provides the ability to add [Intercom](https://www.intercom.com/) to your .NET MAUI application using [Native Library Interop](https://github.com/CommunityToolkit/Maui.NativeLibraryInterop) (NLI).

## Status

Both Android and iOS platforms are working.

<img width="403" height="696" alt="Screenshot 2026-01-20 134953" src="https://github.com/user-attachments/assets/9696d97e-87a2-450a-bd76-ed261101f2f0" />
<img width="395" height="505" alt="Screenshot 2026-01-20 124137" src="https://github.com/user-attachments/assets/c4f5a049-cdbe-46fc-bce4-bc1b4260c8d2" />



## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.Intercom.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.Intercom/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.Intercom).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.Intercom`, or through the NuGet Package Manager in Visual Studio.

### Supported Platforms

| Platform | Minimum Version Supported |
|----------|---------------------------|
| iOS      | 15+                       |
| Android  | 5.0 (API 21)              |

### Native SDK Versions

| Platform | Intercom SDK Version |
|----------|---------------------|
| Android  | 17.4.1              |
| iOS      | Latest via SPM      |

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

No additional configuration is required for iOS.

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

#### Show Messenger

```csharp
// Show messenger without a pre-filled message
Intercom.Default.PresentMessenger(null);

// Show messenger with a pre-filled message
Intercom.Default.PresentMessenger("I need help with...");
```

#### Show Help Center

```csharp
Intercom.Default.PresentHelpCenter();
```

#### Show Support Center

```csharp
Intercom.Default.PresentSupportCenter();
```

#### Show a Carousel

```csharp
Intercom.Default.PresentCarousel("carousel-id");
```

### Customization

#### Show/Hide Messenger Launcher

```csharp
Intercom.Default.SetVisible(true);  // Show launcher
Intercom.Default.SetVisible(false); // Hide launcher
```

#### Adjust Bottom Padding

```csharp
// Add 100 pixels of padding from the bottom
Intercom.Default.SetBottomPadding(100);
```

### Logout

```csharp
Intercom.Default.Logout();
```

## Building from Source

### Prerequisites (Windows)

**⚠️ IMPORTANT:** On Windows, you must enable long path support to build the iOS bindings.

1. Open PowerShell as Administrator
2. Run: `New-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem' -Name 'LongPathsEnabled' -Value 1 -PropertyType DWORD -Force`
3. Restart your computer

### Quick Build

From the repository root:

```powershell
.\build.ps1
```

### Manual Build

```powershell
cd src
dotnet build Plugin.Maui.Intercom.sln
```

For detailed build instructions, troubleshooting, and development tips, see [BUILD.md](src/BUILD.md).

### Dependency Injection

You can also use dependency injection:

```csharp
// In MauiProgram.cs
builder.Services.AddSingleton<IIntercom>(Intercom.Default);

// In your ViewModel or Page
public class MyViewModel
{
    private readonly IIntercom _intercom;

    public MyViewModel(IIntercom intercom)
    {
        _intercom = intercom;
    }

    public void ShowMessenger()
    {
        _intercom.PresentMessenger(null);
    }
}
```

## Complete Example

```csharp
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Initialize Intercom
        Intercom.Default.Initialize("your-api-key", "your-app-id");
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {
        // Register user after login
        Intercom.Default.RegisterWithUserId(
            "user-123",
            onSuccess: () =>
            {
                // Optionally set user hash for identity verification
                Intercom.Default.SetUserHash("server-generated-hash");
            },
            onFailure: (error) =>
            {
                DisplayAlert("Error", $"Failed to register: {error}", "OK");
            }
        );
    }

    private void OnHelpClicked(object sender, EventArgs e)
    {
        Intercom.Default.PresentMessenger(null);
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        Intercom.Default.Logout();
    }
}
```

## Troubleshooting

### Android: Compose Version Mismatch

If you encounter runtime crashes related to `NoSuchMethodError` in Compose classes, ensure you're using Intercom SDK 17.4.1 or later, which is compatible with AndroidX Compose BOM 2025.11.01.

### iOS: Build on Windows

iOS builds require macOS. The sample project is configured to skip iOS targets on Windows to avoid build errors.

## Acknowledgements

This project could not have come to be without these projects and people, thank you!

- [.NET MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui) - For the Native Library Interop pattern
- [Intercom](https://www.intercom.com/) - For their excellent customer messaging platform
