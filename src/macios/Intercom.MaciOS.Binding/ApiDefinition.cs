using System;
using Foundation;
using ObjCRuntime;

namespace MauiIntercomMaciOS
{
	// @interface CompanyWrapper : NSObject
	[BaseType (typeof(NSObject))]
	interface CompanyWrapper
	{
		// @property (copy, nonatomic) NSString * _Nullable companyId;
		[NullAllowed, Export ("companyId")]
		string CompanyId { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }
	}

	// @interface IntercomBinding : NSObject
	[BaseType (typeof(NSObject))]
	interface IntercomBinding
	{
		// +(UserAttributesWrapper * _Nonnull)createUserAttributes __attribute__((warn_unused_result("")));
		[Static]
		[Export ("createUserAttributes")]
		UserAttributesWrapper CreateUserAttributes { get; }

		// +(CompanyWrapper * _Nonnull)createCompanyWithCompanyId:(NSString * _Nonnull)companyId name:(NSString * _Nonnull)name __attribute__((warn_unused_result("")));
		[Static]
		[Export ("createCompanyWithCompanyId:name:")]
		CompanyWrapper CreateCompanyWithCompanyId (string companyId, string name);

		// +(void)loginUnidentifiedUserWithCompletion:(void (^ _Nullable)(BOOL, NSError * _Nullable))completion;
		[Static]
		[Export ("loginUnidentifiedUserWithCompletion:")]
		void LoginUnidentifiedUserWithCompletion ([NullAllowed] Action<bool, NSError> completion);

		// +(void)loginUserWith:(UserAttributesWrapper * _Nonnull)userAttributes completion:(void (^ _Nullable)(BOOL, NSError * _Nullable))completion;
		[Static]
		[Export ("loginUserWith:completion:")]
		void LoginUserWith (UserAttributesWrapper userAttributes, [NullAllowed] Action<bool, NSError> completion);

		// +(void)loginUserWithUserId:(NSString * _Nonnull)userId completion:(void (^ _Nullable)(BOOL, NSError * _Nullable))completion;
		[Static]
		[Export ("loginUserWithUserId:completion:")]
		void LoginUserWithUserId (string userId, [NullAllowed] Action<bool, NSError> completion);

		// +(void)loginUserWithEmail:(NSString * _Nonnull)email completion:(void (^ _Nullable)(BOOL, NSError * _Nullable))completion;
		[Static]
		[Export ("loginUserWithEmail:completion:")]
		void LoginUserWithEmail (string email, [NullAllowed] Action<bool, NSError> completion);

		// +(void)loginUserWithUserId:(NSString * _Nonnull)userId email:(NSString * _Nonnull)email completion:(void (^ _Nullable)(BOOL, NSError * _Nullable))completion;
		[Static]
		[Export ("loginUserWithUserId:email:completion:")]
		void LoginUserWithUserId (string userId, string email, [NullAllowed] Action<bool, NSError> completion);

		// +(void)logout;
		[Static]
		[Export ("logout")]
		void Logout ();

		// +(void)setUserHash:(NSString * _Nullable)userHash;
		[Static]
		[Export ("setUserHash:")]
		void SetUserHash ([NullAllowed] string userHash);

		// +(void)updateUser:(UserAttributesWrapper * _Nonnull)userAttributes completion:(void (^ _Nullable)(BOOL, NSError * _Nullable))completion;
		[Static]
		[Export ("updateUser:completion:")]
		void UpdateUser (UserAttributesWrapper userAttributes, [NullAllowed] Action<bool, NSError> completion);

		// +(void)logEventWithName:(NSString * _Nonnull)name;
		[Static]
		[Export ("logEventWithName:")]
		void LogEventWithName (string name);

		// +(void)logEventWithName:(NSString * _Nonnull)name metaData:(NSDictionary<NSString *,id> * _Nullable)metaData;
		[Static]
		[Export ("logEventWithName:metaData:")]
		void LogEventWithName (string name, [NullAllowed] NSDictionary<NSString, NSObject> metaData);

		// +(void)presentIntercom;
		[Static]
		[Export ("presentIntercom")]
		void PresentIntercom ();

		// +(void)presentIntercomWithSpace:(NSInteger)space;
		[Static]
		[Export ("presentIntercomWithSpace:")]
		void PresentIntercomWithSpace (nint space);

		// +(void)setApiKeyWith:(NSString * _Nonnull)apiKey appId:(NSString * _Nonnull)appId;
		[Static]
		[Export ("setApiKeyWith:appId:")]
		void SetApiKeyWith (string apiKey, string appId);

		// +(void)presentMessageComposer:(NSString * _Nonnull)initialMessage;
		[Static]
		[Export ("presentMessageComposer:")]
		void PresentMessageComposer (string initialMessage);

		// +(void)presentContentWith:(NSString * _Nonnull)contentId contentType:(NSString * _Nonnull)contentType;
		[Static]
		[Export ("presentContentWith:contentType:")]
		void PresentContentWith (string contentId, string contentType);

		// +(void)hideIntercom;
		[Static]
		[Export ("hideIntercom")]
		void HideIntercom ();

		// +(void)setLauncherVisible:(BOOL)visible;
		[Static]
		[Export ("setLauncherVisible:")]
		void SetLauncherVisible (bool visible);

		// +(void)setBottomPadding:(CGFloat)bottomPadding;
		[Static]
		[Export ("setBottomPadding:")]
		void SetBottomPadding (nfloat bottomPadding);

		// +(void)setInAppMessagesVisible:(BOOL)visible;
		[Static]
		[Export ("setInAppMessagesVisible:")]
		void SetInAppMessagesVisible (bool visible);

		// +(void)setNeedsStatusBarAppearanceUpdate;
		[Static]
		[Export ("setNeedsStatusBarAppearanceUpdate")]
		void SetNeedsStatusBarAppearanceUpdate ();

		// +(NSUInteger)unreadConversationCount __attribute__((warn_unused_result("")));
		[Static]
		[Export ("unreadConversationCount")]
		nuint UnreadConversationCount { get; }

		// +(void)enableLogging;
		[Static]
		[Export ("enableLogging")]
		void EnableLogging ();
	}

	// @interface IntercomSpaceBinding : NSObject
	[BaseType (typeof(NSObject))]
	interface IntercomSpaceBinding
	{
		// @property (readonly, nonatomic, class) NSInteger Home;
		[Static]
		[Export ("Home")]
		nint Home { get; }

		// @property (readonly, nonatomic, class) NSInteger HelpCenter;
		[Static]
		[Export ("HelpCenter")]
		nint HelpCenter { get; }

		// @property (readonly, nonatomic, class) NSInteger Messages;
		[Static]
		[Export ("Messages")]
		nint Messages { get; }

		// @property (readonly, nonatomic, class) NSInteger Tickets;
		[Static]
		[Export ("Tickets")]
		nint Tickets { get; }
	}

	// @interface UserAttributesWrapper : NSObject
	[BaseType (typeof(NSObject))]
	interface UserAttributesWrapper
	{
		// @property (copy, nonatomic) NSString * _Nullable userId;
		[NullAllowed, Export ("userId")]
		string UserId { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable phone;
		[NullAllowed, Export ("phone")]
		string Phone { get; set; }

		// @property (copy, nonatomic) NSDictionary<NSString *,id> * _Nullable customAttributes;
		[NullAllowed, Export ("customAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> CustomAttributes { get; set; }

		// @property (copy, nonatomic) NSArray<CompanyWrapper *> * _Nullable companies;
		[NullAllowed, Export ("companies", ArgumentSemantic.Copy)]
		CompanyWrapper[] Companies { get; set; }
	}
}
