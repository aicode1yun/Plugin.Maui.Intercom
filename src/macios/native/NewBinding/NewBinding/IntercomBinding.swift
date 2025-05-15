//
//  IntercomBinding.swift
//  IntercomBindings
//
//

import Foundation
import Intercom

@objc(UserAttributesWrapper)
public class UserAttributesWrapper : NSObject {
    @objc public var userId: String?
    @objc public var email: String?
    @objc public var name: String?
    @objc public var phone: String?
    @objc public var customAttributes: [String: Any]?
    @objc public var companies: [CompanyWrapper]?
}

@objc(CompanyWrapper)
public class CompanyWrapper : NSObject {
    @objc public var companyId: String?
    @objc public var name: String?
}

@objc(IntercomBinding)
public class IntercomBinding : NSObject {
    
    // MARK: - User Attributes
    
    @objc
    public static func createUserAttributes() -> UserAttributesWrapper {
        return UserAttributesWrapper()
    }
    
    @objc
    public static func createCompany(companyId: String, name: String) -> CompanyWrapper {
        let company = CompanyWrapper()
        company.companyId = companyId
        company.name = name
        return company
    }
    
    // MARK: - User Login
    
    @objc
    public static func loginUnidentifiedUser(completion: ((Bool, NSError?) -> Void)?) {
        Intercom.loginUnidentifiedUser { result in
            switch result {
            case .success:
                completion?(true, nil)
            case .failure(let error):
                completion?(false, error as NSError)
            }
        }
    }
    
    @objc
    public static func loginUser(with userAttributes: UserAttributesWrapper, completion: ((Bool, NSError?) -> Void)?) {
        // Convert our wrapper to Intercom's internal type
        let intercomUserAttributes = ICMUserAttributes()
        
        if let userId = userAttributes.userId {
            intercomUserAttributes.userId = userId
        }
        if let email = userAttributes.email {
            intercomUserAttributes.email = email
        }
        if let name = userAttributes.name {
            intercomUserAttributes.name = name
        }
        if let phone = userAttributes.phone {
            intercomUserAttributes.phone = phone
        }
        if let customAttributes = userAttributes.customAttributes {
            intercomUserAttributes.customAttributes = customAttributes
        }
        
        Intercom.loginUser(with: intercomUserAttributes) { result in
            switch result {
            case .success:
                completion?(true, nil)
            case .failure(let error):
                completion?(false, error as NSError)
            }
        }
    }
    
    @objc
    public static func loginUser(userId: String, completion: ((Bool, NSError?) -> Void)?) {
        let userAttributes = UserAttributesWrapper()
        userAttributes.userId = userId
        loginUser(with: userAttributes, completion: completion)
    }
    
    @objc
    public static func loginUser(email: String, completion: ((Bool, NSError?) -> Void)?) {
        let userAttributes = UserAttributesWrapper()
        userAttributes.email = email
        loginUser(with: userAttributes, completion: completion)
    }
    
    @objc
    public static func loginUser(userId: String, email: String, completion: ((Bool, NSError?) -> Void)?) {
        let userAttributes = UserAttributesWrapper()
        userAttributes.userId = userId
        userAttributes.email = email
        loginUser(with: userAttributes, completion: completion)
    }
    
    @objc
    public static func logout() {
        Intercom.logout()
    }
    
    /**
     * Update the user hash, call this before registering a user
     * @param userHash The user hash (HMAC of user id or email)
     */
    @objc
    public static func setUserHash(_ userHash: String?) {
        guard let userHash = userHash else { return }
        Intercom.setUserHash(userHash)
    }
    
    // MARK: - Update User
    
    @objc
    public static func updateUser(_ userAttributes: UserAttributesWrapper, completion: ((Bool, NSError?) -> Void)?) {
        // Convert our wrapper to Intercom's internal type
        let intercomUserAttributes = ICMUserAttributes()
        
        if let userId = userAttributes.userId {
            intercomUserAttributes.userId = userId
        }
        if let email = userAttributes.email {
            intercomUserAttributes.email = email
        }
        if let name = userAttributes.name {
            intercomUserAttributes.name = name
        }
        if let phone = userAttributes.phone {
            intercomUserAttributes.phone = phone
        }
        if let customAttributes = userAttributes.customAttributes {
            intercomUserAttributes.customAttributes = customAttributes
        }
        
        Intercom.updateUser(with: intercomUserAttributes) { result in
            switch result {
            case .success:
                completion?(true, nil)
            case .failure(let error):
                completion?(false, error as NSError)
            }
        }
    }
    
    // MARK: - Events
    
    @objc
    public static func logEvent(name: String) {
        Intercom.logEvent(withName: name)
    }
    
    @objc
    public static func logEvent(name: String, metaData: [String: Any]?) {
        if let metaData = metaData {
            Intercom.logEvent(withName: name, metaData: metaData)
        } else {
            Intercom.logEvent(withName: name)
        }
    }
    
    // MARK: - Presenting Intercom
    
    @objc
    public static func presentIntercom() {
        Intercom.present()
    }
    
    @objc
    public static func presentIntercom(space: Int) {
        if let spaceValue = Space(rawValue: space) {
            Intercom.present(spaceValue)
        }
    }
    
    @objc
    public static func setApiKey(with apiKey: String, appId: String) {
        Intercom.setApiKey(apiKey, forAppId: appId)
    }
    
    @objc
    public static func presentMessageComposer(_ initialMessage: String) {
        Intercom.presentMessageComposer(initialMessage)
    }
    
    @objc
    public static func presentContent(with contentId: String, contentType: String) {
        var content: Intercom.Content?
        
        switch contentType {
        case "article":
            content = Intercom.Content.article(id: contentId)
        case "survey":
            content = Intercom.Content.survey(id: contentId)
        case "carousel":
            content = Intercom.Content.carousel(id: contentId)
        case "helpCenterCollection":
            content = Intercom.Content.helpCenterCollections(ids: [contentId])
        case "conversation":
            content = Intercom.Content.conversation(id: contentId)
        default:
            print("Unknown content type: \(contentType)")
            return
        }
        
        if let content = content {
            Intercom.presentContent(content)
        }
    }
    
    @objc
    public static func hideIntercom() {
        Intercom.hide()
    }
    
    // MARK: - UI Customization
    
    @objc
    public static func setLauncherVisible(_ visible: Bool) {
        Intercom.setLauncherVisible(visible)
    }
    
    @objc
    public static func setBottomPadding(_ bottomPadding: CGFloat) {
        Intercom.setBottomPadding(bottomPadding)
    }
    
    @objc
    public static func setInAppMessagesVisible(_ visible: Bool) {
        Intercom.setInAppMessagesVisible(visible)
    }
    
    @objc
    public static func setNeedsStatusBarAppearanceUpdate() {
        Intercom.setNeedsStatusBarAppearanceUpdate()
    }
    
    // MARK: - Unread Conversation Count
    
    @objc
    public static func unreadConversationCount() -> UInt {
        return Intercom.unreadConversationCount()
    }
    
    // MARK: - Logging
    
    @objc
    public static func enableLogging() {
        Intercom.enableLogging()
    }
}

// MARK: - Spaces Enum Helper

@objc(IntercomSpaceBinding)
public class IntercomSpaceBinding : NSObject {
    @objc public static let Home = Space.home.rawValue
    @objc public static let HelpCenter = Space.helpCenter.rawValue
    @objc public static let Messages = Space.messages.rawValue
    @objc public static let Tickets = Space.tickets.rawValue
}
