using System;
using Foundation;
using ObjCRuntime;

namespace MauiIntercomMaciOS
{
    // @interface CompanyWrapper : NSObject
    [BaseType(typeof(NSObject))]
    public interface CompanyWrapper
    {
        // @property (copy, nonatomic) NSString * _Nullable companyId;
        [NullAllowed, Export("companyId")]
        string CompanyId { get; set; }

        // @property (copy, nonatomic) NSString * _Nullable name;
        [NullAllowed, Export("name")]
        string Name { get; set; }
    }

    // @interface IntercomBinding : NSObject
    [BaseType(typeof(NSObject))]
    public interface IntercomBinding
    {
        // @objc public static func createUserAttributes() -> UserAttributesWrapper
        [Static]
        [Export("createUserAttributes")]
        UserAttributesWrapper CreateUserAttributes();

        // @objc public static func createCompany(companyId: String, name: String) -> CompanyWrapper
        [Static]
        [Export("createCompanyWithCompanyId:name:")]
        CompanyWrapper CreateCompanyWithCompanyId(string companyId, string name);

        // @objc public static func loginUnidentifiedUser(completion: ((Bool, NSError?) -> Void)?)
        [Static]
        [Export("loginUnidentifiedUserWithCompletion:")]
        void LoginUnidentifiedUserWithCompletion([NullAllowed] Action<bool, NSError> completion);

        // @objc public static func loginUser(with userAttributes: UserAttributesWrapper, completion: ((Bool, NSError?) -> Void)?)
        [Static]
        [Export("loginUserWith:completion:")]
        void LoginUserWith(UserAttributesWrapper userAttributes, [NullAllowed] Action<bool, NSError> completion);

        // @objc public static func loginUser(userId: String, completion: ((Bool, NSError?) -> Void)?)
        [Static]
        [Export("loginUserWithUserId:completion:")]
        void LoginUserWithUserId(string userId, [NullAllowed] Action<bool, NSError> completion);

        // @objc public static func loginUser(email: String, completion: ((Bool, NSError?) -> Void)?)
        [Static]
        [Export("loginUserWithEmail:completion:")]
        void LoginUserWithEmail(string email, [NullAllowed] Action<bool, NSError> completion);

        // @objc public static func loginUser(userId: String, email: String, completion: ((Bool, NSError?) -> Void)?)
        [Static]
        [Export("loginUserWithUserId:email:completion:")]
        void LoginUserWithUserId(string userId, string email, [NullAllowed] Action<bool, NSError> completion);

        // @objc public static func logout()
        [Static]
        [Export("logout")]
        void Logout();

        // @objc public static func updateUser(_ userAttributes: UserAttributesWrapper, completion: ((Bool, NSError?) -> Void)?)
        [Static]
        [Export("updateUser:completion:")]
        void UpdateUser(UserAttributesWrapper userAttributes, [NullAllowed] Action<bool, NSError> completion);

        // @objc public static func logEvent(name: String)
        [Static]
        [Export("logEventWithName:")]
        void LogEventWithName(string name);

        // @objc public static func logEvent(name: String, metaData: [String: Any]?)
        [Static]
        [Export("logEventWithName:metaData:")]
        void LogEventWithName(string name, [NullAllowed] NSDictionary<NSString, NSObject> metaData);

        // @objc public static func presentIntercom()
        [Static]
        [Export("presentIntercom")]
        void PresentIntercom();

        // @objc public static func presentIntercom(space: Int)
        [Static]
        [Export("presentIntercomWithSpace:")]
        void PresentIntercomWithSpace(nint space);

        // @objc public static func setApiKey(with apiKey: String, appId: String)
        [Static]
        [Export("setApiKeyWith:appId:")]
        void SetApiKeyWith(string apiKey, string appId);

        // @objc public static func presentMessageComposer(_ initialMessage: String)
        [Static]
        [Export("presentMessageComposer:")]
        void PresentMessageComposer(string initialMessage);

        // @objc public static func presentContent(with contentId: String, contentType: String)
        [Static]
        [Export("presentContentWith:contentType:")]
        void PresentContentWith(string contentId, string contentType);

        // @objc public static func hideIntercom()
        [Static]
        [Export("hideIntercom")]
        void HideIntercom();

        // @objc public static func setLauncherVisible(_ visible: Bool)
        [Static]
        [Export("setLauncherVisible:")]
        void SetLauncherVisible(bool visible);

        // @objc public static func setBottomPadding(_ bottomPadding: CGFloat)
        [Static]
        [Export("setBottomPadding:")]
        void SetBottomPadding(nfloat bottomPadding);

        // @objc public static func setInAppMessagesVisible(_ visible: Bool)
        [Static]
        [Export("setInAppMessagesVisible:")]
        void SetInAppMessagesVisible(bool visible);

        // @objc public static func setNeedsStatusBarAppearanceUpdate()
        [Static]
        [Export("setNeedsStatusBarAppearanceUpdate")]
        void SetNeedsStatusBarAppearanceUpdate();

        // @objc public static func unreadConversationCount() -> UInt
        [Static]
        [Export("unreadConversationCount")]
        nuint UnreadConversationCount();

        // @objc public static func enableLogging()
        [Static]
        [Export("enableLogging")]
        void EnableLogging();
    }

    // @interface IntercomSpaceBinding : NSObject
    [BaseType(typeof(NSObject))]
    public interface IntercomSpaceBinding
    {
        // @objc public static let Home = Space.home.rawValue
        [Static]
        [Export("Home")]
        nint Home { get; }

        // @objc public static let HelpCenter = Space.helpCenter.rawValue
        [Static]
        [Export("HelpCenter")]
        nint HelpCenter { get; }

        // @objc public static let Messages = Space.messages.rawValue
        [Static]
        [Export("Messages")]
        nint Messages { get; }

        // @objc public static let Tickets = Space.tickets.rawValue
        [Static]
        [Export("Tickets")]
        nint Tickets { get; }
    }

    // @interface UserAttributesWrapper : NSObject
    [BaseType(typeof(NSObject))]
    public interface UserAttributesWrapper
    {
        // @objc public var userId: String?
        [NullAllowed, Export("userId")]
        string UserId { get; set; }

        // @objc public var email: String?
        [NullAllowed, Export("email")]
        string Email { get; set; }

        // @objc public var name: String?
        [NullAllowed, Export("name")]
        string Name { get; set; }

        // @objc public var phone: String?
        [NullAllowed, Export("phone")]
        string Phone { get; set; }

        // @objc public var customAttributes: [String: Any]?
        [NullAllowed, Export("customAttributes", ArgumentSemantic.Copy)]
        NSDictionary<NSString, NSObject> CustomAttributes { get; set; }

        // @objc public var companies: [CompanyWrapper]?
        [NullAllowed, Export("companies", ArgumentSemantic.Copy)]
        CompanyWrapper[] Companies { get; set; }
    }
}
