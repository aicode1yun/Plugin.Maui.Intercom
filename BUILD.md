# Building Plugin.Maui.Intercom

This document provides guidance for building the Plugin.Maui.Intercom solution.

## Prerequisites

### Windows Requirements

**⚠️ IMPORTANT: Long Path Support Required**

The iOS binding project uses Swift frameworks with deep directory structures that exceed Windows' default 260-character path limit. You **must** enable long path support before building on Windows.

#### Enable Long Paths (One-time setup):

1. Open PowerShell as **Administrator**
2. Run the following command:
   ```powershell
   New-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem' -Name 'LongPathsEnabled' -Value 1 -PropertyType DWORD -Force
   ```
3. **Restart your computer** for the changes to take effect

Alternatively, you can enable it via Group Policy:
- Run `gpedit.msc`
- Navigate to: Local Computer Policy > Computer Configuration > Administrative Templates > System > Filesystem
- Enable "Enable Win32 long paths"
- Restart your computer

### macOS Requirements

No special configuration needed - long paths are supported by default.

## Building the Solution

### Option 1: Quick Build Script (Easiest)

From the repository root, run the PowerShell build script:

```powershell
.\build.ps1
```

This script automatically builds all projects in the correct order and provides clear progress feedback.

### Option 2: Command Line

The most reliable way to build the solution is via command line:

```powershell
# Navigate to the src directory
cd src

# Build the main library project (builds both iOS and Android)
dotnet build Plugin.Maui.Intercom\Plugin.Maui.Intercom.csproj

# Or build specific target frameworks
dotnet build Plugin.Maui.Intercom\Plugin.Maui.Intercom.csproj -f net9.0-ios
dotnet build Plugin.Maui.Intercom\Plugin.Maui.Intercom.csproj -f net9.0-android

# Build the entire solution
dotnet build Plugin.Maui.Intercom.sln
```

### Option 3: Visual Studio

Open `Plugin.Maui.Intercom.sln` in Visual Studio.

**⚠️ Known Limitation:** Multi-targeted .NET MAUI projects have issues with Visual Studio's "Any CPU" build configuration, which can cause:
- File copy errors for iOS Swift module resources
- Build dependency resolution issues
- Metadata file not found errors

**✅ Recommended Workarounds:**

1. **Before opening in Visual Studio for the first time**, run the build script from repository root:
   ```powershell
   .\build.ps1
   ```
   This ensures all binding projects are built before Visual Studio attempts to build them.

2. **For ongoing development**, use the command line to build after making changes:
   ```powershell
   cd src
   dotnet build Plugin.Maui.Intercom\Plugin.Maui.Intercom.csproj -f net9.0-ios
   dotnet build Plugin.Maui.Intercom\Plugin.Maui.Intercom.csproj -f net9.0-android
   ```

3. **If you encounter build errors in VS**:
   - Close Visual Studio
   - Run `dotnet clean` from the src directory
   - Run the `.\build.ps1` script
   - Reopen Visual Studio

Once the binding projects are built successfully once, IntelliSense and code navigation will work properly in Visual Studio, even if rebuilding has issues.

**Note:** The solution is configured to build binding projects before the main library project automatically. The first build after cleaning may take longer as it needs to build:
1. `Intercom.MaciOS.Binding` (iOS bindings)
2. `Intercom.Android.Binding` (Android bindings)
3. `Plugin.Maui.Intercom` (Main library)
4. `MauiSample` (Sample app)

## Project Structure

```
src/
├── Plugin.Maui.Intercom/          # Main cross-platform library
├── android/
│   └── Intercom.Android.Binding/  # Android native bindings
├── macios/
│   └── Intercom.MaciOS.Binding/   # iOS native bindings
└── sample/
    └── MauiSample/                # Sample application
```

## Common Build Issues

### Issue: "Could not copy the file...swiftinterface because it was not found"

**Cause:** This occurs when long paths are not enabled on Windows or when building binding projects in the wrong order.

**Solution:**
1. Ensure long paths are enabled (see Prerequisites above)
2. Clean the solution: `dotnet clean`
3. Rebuild from command line: `dotnet build`

### Issue: "Metadata file could not be found"

**Cause:** Build dependency timing issue, usually occurs when building in Visual Studio with "Any CPU" configuration.

**Solution:**
1. Clean the solution
2. Build the binding projects first:
   ```powershell
   dotnet build macios\Intercom.MaciOS.Binding\Intercom.MaciOS.Binding.csproj
   dotnet build android\Intercom.Android.Binding\Intercom.Android.Binding.csproj
   ```
3. Then build the main solution

### Issue: Package version warnings (NU1608)

**Status:** These are expected warnings and can be safely ignored. They occur because some AndroidX packages have version constraints that are slightly mismatched.

## CI/CD

The project includes GitHub Actions workflows in `.github/workflows/`:
- `ci.yml` - Continuous integration build
- `ci-sample.yml` - Sample app build
- `release-nuget.yml` - NuGet package release

## Development Tips

1. **Clean builds:** When switching between branches or after pulling changes, run a clean build:
   ```powershell
   dotnet clean && dotnet build
   ```

2. **Build specific platforms:** To speed up builds during development:
   ```powershell
   # iOS only
   dotnet build -f net9.0-ios
   
   # Android only
   dotnet build -f net9.0-android
   ```

3. **IntelliSense issues:** If IntelliSense isn't working properly in Visual Studio:
   - Close Visual Studio
   - Delete all `bin` and `obj` folders
   - Reopen solution and rebuild

## Need Help?

If you encounter build issues not covered here:
1. Check that long paths are enabled (Windows)
2. Try a clean build from command line
3. Check the GitHub Issues for similar problems
4. Open a new issue with your build output
