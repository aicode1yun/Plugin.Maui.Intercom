using System.Security.Cryptography;
using System.Text;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Plugin.Maui.Intercom;

namespace MauiSample;

public partial class MainPage : ContentPage
{
    private readonly IConfiguration _configuration;
    private bool _initialized;

    public MainPage(IConfiguration configuration)
    {
        InitializeComponent();
        _configuration = configuration;
    }

    private static IIntercom Intercom => Ioc.Default.GetRequiredService<IIntercom>();

    private void SetStatus(string message)
    {
        MainThread.BeginInvokeOnMainThread(() => StatusLabel.Text = message);
    }

    private static string GetHmac(string key, string message)
    {
        var encoding = new UTF8Encoding();
        using var hash = new HMACSHA256(encoding.GetBytes(key));
        return Convert.ToHexStringLower(hash.ComputeHash(encoding.GetBytes(message)));
    }

    private (string apiKey, string appId, string secret) GetCredentials()
    {
#if ANDROID
        var apiKey = _configuration.GetValue("Intercom:DroidApiKey", string.Empty);
        var secret = _configuration.GetValue("Intercom:DroidSecret", string.Empty);
#elif IOS
        var apiKey = _configuration.GetValue("Intercom:AppleApiKey", string.Empty);
        var secret = _configuration.GetValue("Intercom:AppleSecret", string.Empty);
#else
        var apiKey = string.Empty;
        var secret = string.Empty;
#endif
        var appId = _configuration.GetValue("Intercom:AppId", string.Empty);
        return (apiKey ?? string.Empty, appId ?? string.Empty, secret ?? string.Empty);
    }

    private bool EnsureInitialized()
    {
        if (_initialized)
        {
            return true;
        }

        SetStatus("Not initialized — tap Initialize first");
        return false;
    }

    private void OnInitializeClicked(object sender, EventArgs e)
    {
        try
        {
            var (apiKey, appId, _) = GetCredentials();
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(appId))
            {
                SetStatus("Missing credentials — set Intercom:AppleApiKey/DroidApiKey and Intercom:AppId in appsettings.Development.json");
                return;
            }

            Intercom.Initialize(apiKey, appId);
            _initialized = true;
            SetStatus("Initialized");
        }
        catch (Exception ex)
        {
            SetStatus($"Initialize failed: {ex.Message}");
        }
    }

    private void OnRegisterUnidentifiedClicked(object sender, EventArgs e)
    {
        if (!EnsureInitialized())
        {
            return;
        }

        try
        {
            Intercom.Register(
                () => SetStatus("Unidentified registration OK"),
                error => SetStatus($"Unidentified registration failed: {error}"));
        }
        catch (Exception ex)
        {
            SetStatus($"Register failed: {ex.Message}");
        }
    }

    private void OnRegisterIdentifiedClicked(object sender, EventArgs e)
    {
        if (!EnsureInitialized())
        {
            return;
        }

        try
        {
            const string email = "test@test.com";
            var (_, _, secret) = GetCredentials();
            if (!string.IsNullOrEmpty(secret))
            {
                // Only needed when identity verification is enabled for the workspace.
                Intercom.SetUserHash(GetHmac(secret, email));
            }

            Intercom.RegisterWithEmail(email,
                () => SetStatus($"Registered {email}"),
                error => SetStatus($"Registration failed: {error}"));
        }
        catch (Exception ex)
        {
            SetStatus($"Register failed: {ex.Message}");
        }
    }

    private void OnPresentMessengerClicked(object sender, EventArgs e)
    {
        if (!EnsureInitialized())
        {
            return;
        }

        try
        {
            Intercom.PresentMessenger(null);
            SetStatus("Messenger presented");
        }
        catch (Exception ex)
        {
            SetStatus($"Present failed: {ex.Message}");
        }
    }

    private void OnPresentHelpCenterClicked(object sender, EventArgs e)
    {
        if (!EnsureInitialized())
        {
            return;
        }

        try
        {
            Intercom.PresentHelpCenter();
            SetStatus("Help center presented");
        }
        catch (Exception ex)
        {
            SetStatus($"Present failed: {ex.Message}");
        }
    }

    private void OnLogEventClicked(object sender, EventArgs e)
    {
        if (!EnsureInitialized())
        {
            return;
        }

        try
        {
            Intercom.LogEvent("sample_button_tapped");
            SetStatus("Event logged");
        }
        catch (NotSupportedException)
        {
            SetStatus("LogEvent is not supported on this platform");
        }
        catch (Exception ex)
        {
            SetStatus($"LogEvent failed: {ex.Message}");
        }
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        try
        {
            Intercom.Logout();
            SetStatus("Logged out");
        }
        catch (Exception ex)
        {
            SetStatus($"Logout failed: {ex.Message}");
        }
    }
}
