using System;
using Foundation;
using MauiIntercomMaciOS;
using IntercomSdk = MauiIntercomMaciOS.IntercomBinding;
using IntercomSpace = MauiIntercomMaciOS.IntercomSpaceBinding;

namespace Plugin.Maui.Intercom;

partial class IntercomImplementation : IIntercom
{
    public void Initialize(string apiKey, string appId)
    {
        IntercomSdk.SetApiKeyWith(apiKey, appId);
    }

    public void Register(Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        IntercomSdk.LoginUnidentifiedUserWithCompletion((success, error) =>
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

    public void RegisterWithUserId(string userId, Action? onSuccess = null, Action<string?>? onFailure = null)
    {
        IntercomSdk.LoginUserWithUserId(userId, (success, error) =>
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
        IntercomSdk.Logout();
    }

    public void SetUserHash(string userHash)
    {
        IntercomSdk.SetUserHash(userHash);
    }

    public void PresentMessenger(string? message = null)
    {
        if (string.IsNullOrEmpty(message))
        {
            IntercomSdk.PresentIntercom();
        }
        else
        {
            IntercomSdk.PresentMessageComposer(message);
        }
    }

    public void PresentHelpCenter()
    {
        // Present the Help Center space
        IntercomSdk.PresentIntercomWithSpace(IntercomSpace.HelpCenter);
    }

    public void PresentSupportCenter()
    {
        // Present the Home space (which contains all Intercom functionality)
        IntercomSdk.PresentIntercomWithSpace(IntercomSpace.Home);
    }

    public void PresentCarousel(string carouselId)
    {
        IntercomSdk.PresentContentWith(carouselId, "carousel");
    }

    public void SetVisible(bool isVisible)
    {
        IntercomSdk.SetLauncherVisible(isVisible);
    }

    public void SetBottomPadding(int bottomPadding)
    {
        IntercomSdk.SetBottomPadding(bottomPadding);
    }

    // Additional methods that might be useful for your implementation

    public void LogEvent(string eventName)
    {
        IntercomSdk.LogEventWithName(eventName);
    }

    public void LogEvent(string eventName, Dictionary<string, object> metadata)
    {
        var nsDict = metadata.ToNSDictionary();
        IntercomSdk.LogEventWithName(eventName, nsDict);
    }

    public void UpdateUser(string? userId = null, string? email = null, string? name = null,
        string? phone = null, Dictionary<string, object>? customAttributes = null)
    {
        var userAttributes = IntercomSdk.CreateUserAttributes();

        if (!string.IsNullOrEmpty(userId))
            userAttributes.UserId = userId;

        if (!string.IsNullOrEmpty(email))
            userAttributes.Email = email;

        if (!string.IsNullOrEmpty(name))
            userAttributes.Name = name;

        if (!string.IsNullOrEmpty(phone))
            userAttributes.Phone = phone;

        if (customAttributes != null)
            userAttributes.CustomAttributes = customAttributes.ToNSDictionary();

        IntercomSdk.UpdateUser(userAttributes, null);
    }

    public void HideMessenger()
    {
        IntercomSdk.HideIntercom();
    }

    public int GetUnreadConversationCount()
    {
        return (int)IntercomSdk.UnreadConversationCount();
    }
}

// Extension method to convert Dictionary to NSDictionary
public static class DictionaryExtensions
{
    public static NSDictionary<NSString, NSObject> ToNSDictionary(this Dictionary<string, object> dictionary)
    {
        var nsDict = new NSDictionary<NSString, NSObject>();

        foreach (var item in dictionary)
        {
            NSObject nsValue;

            // Convert C# types to NSObject types
            if (item.Value is string strValue)
                nsValue = new NSString(strValue);
            else if (item.Value is int intValue)
                nsValue = new NSNumber(intValue);
            else if (item.Value is double doubleValue)
                nsValue = new NSNumber(doubleValue);
            else if (item.Value is bool boolValue)
                nsValue = new NSNumber(boolValue);
            else if (item.Value is DateTime dateValue)
                nsValue = (NSDate)dateValue;
            else if (item.Value is Dictionary<string, object> dictValue)
                nsValue = dictValue.ToNSDictionary();
            else
                nsValue = NSObject.FromObject(item.Value);

            nsDict.SetValueForKey(nsValue, new NSString(item.Key));
        }

        return nsDict;
    }
}
