package com.intercom.mauiintercom;

import android.app.Activity;
import android.app.Application;
import android.util.Log;

import androidx.annotation.NonNull;

import java.util.Map;
import java.util.Objects;

import io.intercom.android.sdk.Intercom;
import io.intercom.android.sdk.IntercomContent;
import io.intercom.android.sdk.IntercomError;
import io.intercom.android.sdk.IntercomSpace;
import io.intercom.android.sdk.IntercomStatusCallback;
import io.intercom.android.sdk.identity.Registration;

/**
 * Intercom SDK class
 */
public class IntercomSdk {

    /**
     * Turn on Intercom's verbose logging (logcat tag "intercom"). Call before initialize.
     * The Messenger reports most failures as a generic error screen; these logs carry the reason.
     */
    public static void enableLogging() {
        try {
            Intercom.setLogLevel(Intercom.LogLevel.VERBOSE);
        } catch (Exception e) {
            Log.e("intercom", "Enabling Intercom logging failed: " + e.getMessage());
        }
    }

    /**
     * Whether a user is currently logged in to Intercom.
     * @return {boolean} true when a user is registered
     */
    public static boolean isUserLoggedIn() {
        try {
            return Intercom.client().isUserLoggedIn();
        } catch (Exception e) {
            Log.e("intercom", "Checking Intercom login state failed: " + e.getMessage());
            return false;
        }
    }

    /**
     * Log an event with the given name
     * @param name {String} The name of the event
     */
    public static void logEvent(String name) {
        try {
            Intercom.client().logEvent(name);
        } catch (Exception e) {
            Log.e("intercom", "Logging event failed: " + e.getMessage());
        }
    }

    /**
     * Initialize Intercom SDK
     * @param activity {Activity} The activity to initialize the Intercom SDK
     * @param apiKey {String} The Intercom API key
     * @param appId {String} The Intercom app ID
     */
    public static void initialize(Activity activity, String apiKey, String appId) {
        try {
            Application application = activity.getApplication();
            Intercom.initialize(application, apiKey, appId);
        } catch (Exception e) {
            Log.e("intercom", "Intercom initialization failed: " + e.getMessage());
        }
    }

    public static void registerUser(IIntercomCallback callback) {
        registerUser(null, callback);
    }

    /**
     * Register a user with Intercom
     * @param userAttributes {email: String, userId: String} The user attributes to register with Intercom
     * @param callback {IIntercomCallback} The callback to handle the registration result
     */
    public static void registerUser(Map<String, String> userAttributes, IIntercomCallback callback) {
        try {
            if (userAttributes == null) {
                Intercom.client().loginUnidentifiedUser(new IntercomStatusCallback() {
                    @Override
                    public void onSuccess() {
                        Log.d("intercom", "unidentified registration successful");
                        if (callback != null)
                            callback.onSuccess();
                    }

                    @Override
                    public void onFailure(@NonNull IntercomError intercomError) {
                        Log.w("intercom", "unidentified registration failed: " + intercomError.getErrorMessage());
                        if (callback != null)
                            callback.onFailure(intercomError.getErrorMessage());
                    }
                });
            } else {
                boolean hasEmail = userAttributes.containsKey("email") && userAttributes.get("email") != null;
                boolean hasUserId = userAttributes.containsKey("userId") && userAttributes.get("userId") != null;
                Registration userRegistration = Registration.create();
                if (hasEmail) {
                    userRegistration.withEmail(Objects.requireNonNull(userAttributes.get("email")));
                }
                if (hasUserId) {
                    userRegistration.withUserId(Objects.requireNonNull(userAttributes.get("userId")));
                }
                Intercom.client().loginIdentifiedUser(userRegistration, new IntercomStatusCallback() {
                    @Override
                    public void onSuccess() {
                        Log.d("intercom", "identified registration successful");
                        if (callback != null)
                            callback.onSuccess();
                    }

                    @Override
                    public void onFailure(@NonNull IntercomError intercomError) {
                        Log.w("intercom", "identified registration failed: " + intercomError.getErrorMessage());
                        if (callback != null)
                            callback.onFailure(intercomError.getErrorMessage());
                    }
                });
            }
        } catch (Exception e) {
            Log.e("intercom", "Intercom registration failed: " + e.getMessage());
            if (callback != null)
                callback.onFailure(e.getMessage());
        }
    }

    /**
     * Update the user hash, call this before registering a user
     * @param userHash {String} The user hash
     */
    public static void setUserHash(String userHash) {
        try {
            if (userHash != null) {
                Intercom.client().setUserHash(userHash);
            }
        } catch (Exception e) {
            Log.e("intercom", "Intercom user hash failed: " + e.getMessage());
        }
    }

    /**
     * Show the Intercom messenger with an optional message
     * @param message {String} The message to present
     */
    public static void presentMessenger(String message) {
        try {
            if (message != null) {
                Intercom.client().displayMessageComposer(message);
            } else {
                Intercom.client().present();
            }
        } catch (Exception e) {
            Log.e("intercom", "Presenting intercom failed: " + e.getMessage());
        }
    }

    /**
     * Show the Intercom help center
     */
    public static void presentHelpCenter() {
        try {
            Intercom.client().present(IntercomSpace.HelpCenter);
        } catch (Exception e) {
            Log.e("intercom", "Presenting help center failed: " + e.getMessage());
        }
    }

    /**
     * Show the Intercom support center
     */
    public static void presentSupportCenter() {
        try {
            Intercom.client().present(IntercomSpace.Home);
        } catch (Exception e) {
            Log.e("intercom", "Presenting support center failed: " + e.getMessage());
        }
    }

    /**
     * Show the Intercom message composer with a message
     * @param message {String} The message to present
     */
    public static void presentMessageComposer(String message) {
        try {
            presentMessenger(message);
        } catch (Exception e) {
            Log.e("intercom", "Presenting message composer failed: " + e.getMessage());
        }
    }

    /**
     * Show the Intercom carousel with a carousel ID
     * @param carouselId {String} The carousel ID to present
     */
    public static void presentCarousel(String carouselId) {
        try {
            Intercom.client().presentContent(new IntercomContent.Carousel(carouselId));
        } catch (Exception e) {
            Log.e("intercom", "Presenting carousel failed: " + e.getMessage());
        }
    }

    /**
     * Show the Intercom launcher
     * @param isVisible {Boolean} The visibility of the launcher
     */
    public static void setVisible(Boolean isVisible) {
        try {
            Intercom.client().setLauncherVisibility(isVisible ? Intercom.Visibility.VISIBLE : Intercom.Visibility.GONE);
        } catch (Exception e) {
            Log.e("intercom", "Setting launcher visibility failed: " + e.getMessage());
        }
    }

    /**
     * Set the bottom padding of the Intercom messenger
     * @param bottomPadding {Integer} The bottom padding of the messenger
     */
    public static void setBottomPadding(Integer bottomPadding) {
        try {
            Intercom.client().setBottomPadding(bottomPadding);
        } catch (Exception e) {
            Log.e("intercom", "Setting bottom padding failed: " + e.getMessage());
        }
    }

    /**
     * Logout the current user
     */
    public static void logout() {
        try {
            Intercom.client().logout();
        } catch (Exception e) {
            Log.e("intercom", "Logging out failed: " + e.getMessage());
        }
    }
}
