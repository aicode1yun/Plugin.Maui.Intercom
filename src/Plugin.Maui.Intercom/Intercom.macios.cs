using Foundation;
using Microsoft.Maui.ApplicationModel;
using IntercomBinding;
using NativeIntercom = IntercomBinding.Intercom;
using NativeSpace = IntercomBinding.Space;
using NativeContent = IntercomBinding.IntercomContent;

namespace Plugin.Maui.Intercom;

partial class IntercomImplementation : IIntercom
{
    public void Initialize(string apiKey, string appId)
    {
        // Intercom requires initialization on the main thread.
        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.SetApiKey(apiKey, appId));
    }

    public void Register(Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        MainThread.BeginInvokeOnMainThread(() =>
            NativeIntercom.LoginUnidentifiedUserWithSuccess(
                () => onSuccess?.Invoke(),
                error => onFailure?.Invoke(error?.LocalizedDescription)));
    }

    public void RegisterWithUserId(string userId, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException($"'{nameof(userId)}' cannot be null or empty.", nameof(userId));
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            var attributes = new ICMUserAttributes { UserId = userId };
            NativeIntercom.LoginUserWithUserAttributes(
                attributes,
                () => onSuccess?.Invoke(),
                error => onFailure?.Invoke(error?.LocalizedDescription));
        });
    }

    public void RegisterWithEmail(string email, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            var attributes = new ICMUserAttributes { Email = email };
            NativeIntercom.LoginUserWithUserAttributes(
                attributes,
                () => onSuccess?.Invoke(),
                error => onFailure?.Invoke(error?.LocalizedDescription));
        });
    }

    public void Logout()
    {
        MainThread.BeginInvokeOnMainThread(NativeIntercom.Logout);
    }

    public void SetUserHash(string userHash)
    {
        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.SetUserHash(userHash));
    }

    public void PresentMessenger(string? message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (string.IsNullOrEmpty(message))
            {
                NativeIntercom.PresentIntercom();
            }
            else
            {
                NativeIntercom.PresentMessageComposer(message);
            }
        });
    }

    public void PresentHelpCenter()
    {
        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.PresentIntercom(NativeSpace.HelpCenter));
    }

    public void PresentSupportCenter()
    {
        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.PresentIntercom(NativeSpace.Home));
    }

    public void PresentCarousel(string carouselId)
    {
        if (string.IsNullOrEmpty(carouselId))
        {
            throw new ArgumentException($"'{nameof(carouselId)}' cannot be null or empty.", nameof(carouselId));
        }

        MainThread.BeginInvokeOnMainThread(() =>
            NativeIntercom.PresentContent(NativeContent.CarouselWithId(carouselId)));
    }

    public void SetVisible(bool isVisible)
    {
        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.SetLauncherVisible(isVisible));
    }

    public void SetBottomPadding(int bottomPadding)
    {
        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.SetBottomPadding((nfloat)bottomPadding));
    }

    public void LogEvent(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        MainThread.BeginInvokeOnMainThread(() => NativeIntercom.LogEventWithName(name));
    }
}
