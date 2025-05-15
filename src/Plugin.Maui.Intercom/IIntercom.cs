namespace Plugin.Maui.Intercom;

/// <summary>
///     Interface for Intercom plugin.
/// </summary>
public interface IIntercom
{
    /// <summary>
    ///     Initialize Intercom with your API key and App ID.
    /// </summary>
    /// <param name="apiKey">Your Intercom API key.</param>
    /// <param name="appId">Your Intercom App ID.</param>
    void Initialize(string apiKey, string appId);

    /// <summary>
    ///     Register an unidentified user.
    /// </summary>
    /// <param name="onSuccess">
    ///     An optional callback used when the registration is successful.
    /// </param>
    /// <param name="onFailure">
    ///     An optional callback used when the registration is not successful.
    /// </param>
    void Register(Action? onSuccess = null, Action<string?>? onFailure = null);

    /// <summary>
    ///     Register a user using their userId
    /// </summary>
    /// <param name="userId">The userId of the user you want to register</param>
    /// <param name="onSuccess">An optional callback used when the registration is successful</param>
    /// <param name="onFailure">An optional callback used when the registration is not successful</param>
    /// <exception cref="ArgumentException">Thrown when the userId is null or empty</exception>
    void RegisterWithUserId(string userId, Action? onSuccess = null, Action<string?>? onFailure = null);

    /// <summary>
    ///     Register a user using their email
    /// </summary>
    /// <param name="email">The email address of the user you want to register</param>
    /// <param name="onSuccess">An optional callback used when the registration is successful</param>
    /// <param name="onFailure">An optional callback used when the registration is not successful</param>
    /// <exception cref="ArgumentException">Thrown when the email is null or empty</exception>
    void RegisterWithEmail(string email, Action? onSuccess = null, Action<string?>? onFailure = null);

    /// <summary>
    ///     Log out the current user.
    /// </summary>
    void Logout();

    /// <summary>
    ///     Secures the user session with Intercom.
    /// </summary>
    /// <param name="userHash">
    ///     The HMAC hash of the userId and the app secret.
    /// </param>
    void SetUserHash(string userHash);

    /// <summary>
    ///     Show the messenger with a predefined message.
    /// </summary>
    /// <param name="message">
    ///     The message to be displayed in the messenger. If null, the messenger will be shown without a message.
    /// </param>
    void PresentMessenger(string? message);

    /// <summary>
    ///     Show the help center to the user
    /// </summary>
    void PresentHelpCenter();

    /// <summary>
    ///     Show the support center to the user
    /// </summary>
    void PresentSupportCenter();

    /// <summary>
    ///     Show a carousel to the user.
    /// </summary>
    /// <param name="carouselId"></param>
    void PresentCarousel(string carouselId);

    /// <summary>
    ///     Show/Hide the messenger
    /// </summary>
    /// <param name="isVisible"></param>
    void SetVisible(bool isVisible);

    /// <summary>
    ///     Set the distance from the bottom of the screen to the messenger.
    /// </summary>
    /// <param name="bottomPadding"></param>
    void SetBottomPadding(int bottomPadding);
}
