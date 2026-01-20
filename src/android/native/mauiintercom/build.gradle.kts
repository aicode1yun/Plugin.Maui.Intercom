plugins {
    id("com.android.library")
}

android {
    namespace = "com.intercom.mauiintercom"
    compileSdk = 34

    defaultConfig {
        minSdk = 21
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_1_8
        targetCompatibility = JavaVersion.VERSION_1_8
    }
}

dependencies {
    // Use intercom-sdk-base which matches the AAR files in the binding project
    // The UI and other dependencies are provided via the binding project's AndroidLibrary items
    implementation ("io.intercom.android:intercom-sdk-base:17.4.1")
}
