// Supplemental enum definitions for NS_ENUMs the swift-dotnet-bindings ObjC
// pipeline does not yet emit (see ApiDefinitions.extra.cs for context).
// Source of truth: Intercom.h (Space) and IntercomContent.h (ContentType).
using ObjCRuntime;

namespace IntercomBinding
{
    /// <summary>Intercom spaces that can be presented (Intercom.h).</summary>
    [Native]
    public enum Space : long
    {
        Home = 0,
        HelpCenter = 1,
        Messages = 2,
        Tickets = 3,
    }

    /// <summary>Intercom content types (IntercomContent.h).</summary>
    [Native]
    public enum ContentType : long
    {
        Article = 0,
        Survey = 1,
        Carousel = 2,
        HelpCenterCollections = 3,
        Conversation = 4,
    }
}
