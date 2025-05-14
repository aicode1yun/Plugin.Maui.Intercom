using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Plugin.Maui.Intercom;

namespace MauiSample;

public partial class MainPage : ContentPage
{
    private readonly IConfiguration _configuration;

    public MainPage(IConfiguration configuration)
    {
        InitializeComponent();
        _configuration = configuration;
    }

    private static string GetHmac(string key, string message)
    {
        // change according to your needs, an UTF8Encoding
        // could be more suitable in certain situations
        var encoding = new UTF8Encoding();

        var messageBytes = encoding.GetBytes(message);
        var keyBytes = encoding.GetBytes(key);

        byte[] hashBytes;

        using (var hash = new HMACSHA256(keyBytes))
        {
            hashBytes = hash.ComputeHash(messageBytes);
        }

        return Convert.ToHexStringLower(hashBytes);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var intercom = Ioc.Default.GetRequiredService<IIntercom>();
        var intercomApiKey = _configuration.GetValue("Intercom:DroidApiKey", string.Empty);
        var intercomAppId = _configuration.GetValue("Intercom:AppId", string.Empty);

        intercom.Initialize(intercomApiKey, intercomAppId);

        //// If user verification is not on, you don't need to set the user hash
        //intercom.Logout();
        //intercom.RegisterWithEmail("test@test.com");

        //// If user verification is on, you need to set the user hash
        var intercomSecret = _configuration.GetValue("Intercom:DroidSecret", string.Empty);
        //intercom.Logout();
        intercom.SetUserHash(GetHmac(intercomSecret, "test@test.com"));
        intercom.RegisterWithEmail("test@test.com", () =>
        {
            Debug.WriteLine("Intercom Registration SUCCESSFUL");
        }, msg =>
        {
            Debug.WriteLine("Intercom Registration FAILED: '{ErrorMessage}'", msg ?? string.Empty);
        });

        // If there's no user info at all, you can just call register
        //intercom.Logout();
        //intercom.RegisterWithEmail(() =>
        //{
        //    Debug.WriteLine("Intercom Registration SUCCESSFUL");
        //}, (string? msg) =>
        //{
        //    Debug.WriteLine("Intercom Registration FAILED: '{ErrorMessage}'", msg ?? string.Empty);
        //});

        intercom.SetVisible(true);
    }

    private async void OnDocsButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var uri = new Uri(
                "https://learn.microsoft.com/dotnet/communitytoolkit/maui/native-library-interop/get-started");

            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Browser failed to launch. " + ex);
        }
    }

    private async void OnRepoButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var uri = new Uri(
                "https://github.com/kfrancis/Plugin.Maui.Intercom");

            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Browser failed to launch. " + ex);
        }
    }
}
