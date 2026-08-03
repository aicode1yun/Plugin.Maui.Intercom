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

            // Before Initialize: the native SDK then logs why a later Messenger
            // presentation fails instead of only showing its generic error screen.
            Intercom.EnableLogging();
            Intercom.Initialize(apiKey, appId);
            _initialized = true;
            SetStatus($"Initialized (appId {appId})");
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
                () => SetStatus($"Unidentified registration OK (logged in: {Intercom.IsUserLoggedIn})"),
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
                () => SetStatus($"Registered {email} (logged in: {Intercom.IsUserLoggedIn})"),
                error => SetStatus($"Registration failed: {error}"));
        }
        catch (Exception ex)
        {
            SetStatus($"Register failed: {ex.Message}");
        }
    }

    // The Messenger is the heaviest smoke test in the sample: it exercises the native UI
    // stack and the network round-trip, and it reports every failure the same way — a
    // generic "something went wrong" screen. Presenting it without a logged-in user is the
    // most common cause, so register first and say so in the status label.
    private void OnPresentMessengerClicked(object sender, EventArgs e)
    {
        PresentMessenger(null);
    }

    private void OnPresentComposerClicked(object sender, EventArgs e)
    {
        PresentMessenger("Hello from the Plugin.Maui.Intercom smoke test");
    }

    private void PresentMessenger(string? message)
    {
        if (!EnsureInitialized())
        {
            return;
        }

        try
        {
            if (!Intercom.IsUserLoggedIn)
            {
                SetStatus("No user logged in — registering unidentified user first…");
                Intercom.Register(
                    () => Present(message),
                    error => SetStatus($"Messenger skipped — registration failed: {error}"));
                return;
            }

            Present(message);
        }
        catch (Exception ex)
        {
            SetStatus($"Present failed: {ex.Message}");
        }

        void Present(string? initialMessage)
        {
            try
            {
                Intercom.PresentMessenger(initialMessage);
                SetStatus(initialMessage is null
                    ? "Messenger presented (logged in). If it shows an error screen, check the Intercom debug log."
                    : "Message composer presented. If it shows an error screen, check the Intercom debug log.");
            }
            catch (Exception ex)
            {
                SetStatus($"Present failed: {ex.Message}");
            }
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
            SetStatus($"Logged out (logged in: {Intercom.IsUserLoggedIn})");
        }
        catch (Exception ex)
        {
            SetStatus($"Logout failed: {ex.Message}");
        }
    }
}
