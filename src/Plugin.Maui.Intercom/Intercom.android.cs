#if ANDROID
using Java.Lang;
using MauiIntercomAndroid;
using Boolean = Java.Lang.Boolean;
using Object = Java.Lang.Object;

namespace Plugin.Maui.Intercom;

internal class IntercomImplementation : IIntercom
{
    /// <summary>
    ///     Initialize Intercom with your API key and App ID.
    /// </summary>
    /// <param name="apiKey">Your Intercom API key.</param>
    /// <param name="appId">Your Intercom App ID.</param>
    public void Initialize(string apiKey, string appId)
    {
        IntercomSdk.Initialize(Platform.CurrentActivity, apiKey, appId);
    }

    /// <summary>
    ///     Register a user using their userId
    /// </summary>
    /// <param name="userId">The userId of the user you want to register</param>
    /// <param name="onSuccess">An optional callback used when the registration is successful</param>
    /// <param name="onFailure">An optional callback used when the registration is not successful</param>
    /// <exception cref="ArgumentException">Thrown when the userId is null or empty</exception>
    public void RegisterWithUserId(string userId, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException($"'{nameof(userId)}' cannot be null or empty.", nameof(userId));
        }

        var userAttributes = new Dictionary<string, string> { { "userId", userId } };
        IntercomSdk.RegisterUser(userAttributes, new IntercomCallback(onSuccess, onFailure));
    }

    public void Register(Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        IntercomSdk.RegisterUser(new IntercomCallback(onSuccess, onFailure));
    }

    /// <summary>
    ///     Register a user using their email
    /// </summary>
    /// <param name="email">The email address of the user you want to register</param>
    /// <param name="onSuccess">An optional callback used when the registration is successful</param>
    /// <param name="onFailure">An optional callback used when the registration is not successful</param>
    /// <exception cref="ArgumentException">Thrown when the email is null or empty</exception>
    public void RegisterWithEmail(string email, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
        }

        var userAttributes = new Dictionary<string, string> { { "email", email } };
        IIntercomCallback callback = new IntercomCallback(onSuccess, onFailure);
        IntercomSdk.RegisterUser(userAttributes, callback);
    }

    public void SetUserHash(string userHash)
    {
        IntercomSdk.SetUserHash(userHash);
    }

    public void PresentMessenger(string? message)
    {
        IntercomSdk.PresentMessenger(message);
    }

    public void PresentHelpCenter()
    {
        IntercomSdk.PresentHelpCenter();
    }

    public void PresentSupportCenter()
    {
        IntercomSdk.PresentSupportCenter();
    }

    public void PresentCarousel(string carouselId)
    {
        IntercomSdk.PresentCarousel(carouselId);
    }

    public void SetVisible(bool isVisible)
    {
        IntercomSdk.SetVisible(isVisible ? Boolean.True : Boolean.False);
    }

    public void SetBottomPadding(int bottomPadding)
    {
        IntercomSdk.SetBottomPadding(Integer.ValueOf(bottomPadding));
    }

    public void Logout()
    {
        IntercomSdk.Logout();
    }

    private class IntercomCallback : Object, IIntercomCallback
    {
        private readonly Action<string?>? _onFailure;
        private readonly Action? _onSuccess;

        public IntercomCallback(Action? onSuccess, Action<string?>? onFailure)
        {
            _onSuccess = onSuccess;
            _onFailure = onFailure;
        }

        public void OnFailure(string? error)
        {
            _onFailure?.Invoke(error);
        }

        public void OnSuccess()
        {
            _onSuccess?.Invoke();
        }
    }
}
#endif
