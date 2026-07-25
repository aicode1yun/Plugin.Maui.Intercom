// Supplemental Objective-C binding definitions for types the swift-dotnet-bindings
// ObjC pipeline does not yet emit for the Intercom framework: classes declared in
// headers imported by the umbrella header (ICM*, IntercomContent) and the members
// of the Intercom class that reference them (presentIntercom:, presentContent:).
//
// bgen merges these partial interfaces with the generated ApiDefinition.cs.
// Keep this file to the minimum surface the generator misses; re-check it when
// updating the Intercom SDK (eng/update-intercom.sh) — the source of truth is
// Intercom.xcframework/ios-arm64/Intercom.framework/Headers/*.h.
using Foundation;
using ObjCRuntime;

namespace IntercomBinding
{
    // Space and ContentType enums are defined in StructsAndEnums.extra.cs.

    /// <summary>Additional Intercom members whose parameter types live in imported headers.</summary>
    partial interface Intercom
    {
        /// <summary>Present a specific Intercom Space.</summary>
        [Static]
        [Export("presentIntercom:")]
        void PresentIntercom(Space space);

        /// <summary>Present Intercom content (article, survey, carousel, help center collections, conversation).</summary>
        [Static]
        [Export("presentContent:")]
        void PresentContent(IntercomContent content);
    }

    /// <summary>Attributes used to identify and update a user (ICMUserAttributes.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMUserAttributes
    {
        [NullAllowed, Export("email")]
        string Email { get; set; }

        [NullAllowed, Export("userId")]
        string UserId { get; set; }

        [NullAllowed, Export("name")]
        string Name { get; set; }

        [NullAllowed, Export("phone")]
        string Phone { get; set; }

        [NullAllowed, Export("languageOverride")]
        string LanguageOverride { get; set; }

        [NullAllowed, Export("signedUpAt", ArgumentSemantic.Strong)]
        NSDate SignedUpAt { get; set; }

        [Export("unsubscribedFromEmails")]
        bool UnsubscribedFromEmails { get; set; }

        [NullAllowed, Export("companies", ArgumentSemantic.Strong)]
        ICMCompany[] Companies { get; set; }

        [NullAllowed, Export("customAttributes", ArgumentSemantic.Strong)]
        NSDictionary<NSString, NSObject> CustomAttributes { get; set; }

        [Static]
        [Export("nullStringAttribute")]
        string NullStringAttribute { get; }

        [Static]
        [Export("nullNumberAttribute")]
        NSNumber NullNumberAttribute { get; }

        [Static]
        [Export("nullDateAttribute")]
        NSDate NullDateAttribute { get; }
    }

    /// <summary>A company associated with a user (ICMCompany.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMCompany
    {
        [NullAllowed, Export("companyId")]
        string CompanyId { get; set; }

        [NullAllowed, Export("name")]
        string Name { get; set; }

        [NullAllowed, Export("createdAt", ArgumentSemantic.Strong)]
        NSDate CreatedAt { get; set; }

        [NullAllowed, Export("monthlySpend", ArgumentSemantic.Strong)]
        NSNumber MonthlySpend { get; set; }

        [NullAllowed, Export("plan")]
        string Plan { get; set; }

        [NullAllowed, Export("customAttributes", ArgumentSemantic.Strong)]
        NSDictionary<NSString, NSObject> CustomAttributes { get; set; }
    }

    /// <summary>Content descriptor passed to Intercom.PresentContent (IntercomContent.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface IntercomContent
    {
        [Export("type", ArgumentSemantic.Assign)]
        ContentType Type { get; set; }

        [Static]
        [Export("articleWithId:")]
        IntercomContent ArticleWithId(string articleId);

        [Static]
        [Export("carouselWithId:")]
        IntercomContent CarouselWithId(string carouselId);

        [Static]
        [Export("surveyWithId:")]
        IntercomContent SurveyWithId(string surveyId);

        [Static]
        [Export("helpCenterCollectionsWithIds:")]
        IntercomContent HelpCenterCollectionsWithIds(string[] collectionIds);

        [Static]
        [Export("conversationWithId:")]
        IntercomContent ConversationWithId(string conversationId);
    }

    /// <summary>A Help Center collection (ICMHelpCenterCollection.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMHelpCenterCollection
    {
        [Export("collectionId")]
        string CollectionId { get; set; }

        [Export("title")]
        string Title { get; set; }

        [NullAllowed, Export("summary")]
        string Summary { get; set; }

        [Export("articleCount")]
        nint ArticleCount { get; set; }

        [Export("collectionCount")]
        nint CollectionCount { get; set; }
    }

    /// <summary>A Help Center article (ICMHelpCenterArticle.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMHelpCenterArticle
    {
        [Export("articleId")]
        string ArticleId { get; set; }

        [Export("title")]
        string Title { get; set; }
    }

    /// <summary>The author of a Help Center article (ICMHelpCenterArticleAuthor.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMHelpCenterArticleAuthor
    {
        [Export("authorId")]
        string AuthorId { get; set; }

        [Export("displayName")]
        string DisplayName { get; set; }

        [NullAllowed, Export("avatarURL", ArgumentSemantic.Copy)]
        NSUrl AvatarUrl { get; set; }
    }

    /// <summary>A Help Center collection with its contents (ICMHelpCenterCollectionContent.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMHelpCenterCollectionContent
    {
        [Export("collectionId")]
        string CollectionId { get; set; }

        [Export("title")]
        string Title { get; set; }

        [NullAllowed, Export("summary")]
        string Summary { get; set; }

        [Export("articles", ArgumentSemantic.Strong)]
        ICMHelpCenterArticle[] Articles { get; set; }

        [Export("articleCount")]
        nint ArticleCount { get; set; }

        [Export("collections", ArgumentSemantic.Strong)]
        ICMHelpCenterCollection[] Collections { get; set; }

        [Export("authors", ArgumentSemantic.Copy)]
        ICMHelpCenterArticleAuthor[] Authors { get; set; }
    }

    /// <summary>A Help Center search result (ICMHelpCenterArticleSearchResult.h).</summary>
    [BaseType(typeof(NSObject))]
    partial interface ICMHelpCenterArticleSearchResult
    {
        [Export("articleId")]
        string ArticleId { get; set; }

        [Export("title")]
        string Title { get; set; }

        [Export("summary")]
        string Summary { get; set; }

        [Export("matchingSnippet")]
        string MatchingSnippet { get; set; }
    }
}
