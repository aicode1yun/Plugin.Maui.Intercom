using System;
using MauiIntercomMaciOS;
using IntercomSdk = MauiIntercomMaciOS.IntercomBinding;

namespace Plugin.Maui.Intercom;

partial class IntercomImplementation : IIntercom
{
    public void Initialize(string apiKey, string appId)
    {
        IntercomSdk.SetApiKeyWith(apiKey, appId);
    }

    public void Register(Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        throw new NotImplementedException();
    }

    public void RegisterWithUserId(string userId, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        throw new NotImplementedException();
    }

    public void RegisterWithEmail(string email, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        IntercomSdk.LoginUserWithEmail(email, (success, error) =>
        {
            if (success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFailure?.Invoke(error?.LocalizedDescription);
            }
        });
    }

    public void Logout()
    {
        throw new NotImplementedException();
    }

    public void SetUserHash(string userHash)
    {
        // Do nothing
    }

    public void PresentMessenger(string? message)
    {
        throw new NotImplementedException();
    }

    public void PresentHelpCenter()
    {
        throw new NotImplementedException();
    }

    public void PresentSupportCenter()
    {
        throw new NotImplementedException();
    }

    public void PresentCarousel(string carouselId)
    {
        throw new NotImplementedException();
    }

    public void SetVisible(bool isVisible)
    {
        IntercomSdk.SetLauncherVisible(isVisible);
    }

    public void SetBottomPadding(int bottomPadding)
    {
        throw new NotImplementedException();
    }

}
