package com.f8v2;

import com.facebook.react.ReactActivity;
import com.facebook.react.ReactPackage;
import com.facebook.react.shell.MainReactPackage;

import com.facebook.react.bridge.JavaScriptModule;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;
import com.facebook.CallbackManager;

import com.facebook.FacebookSdk;
import com.facebook.appevents.AppEventsLogger;

import android.content.Intent;
import android.os.Bundle;

import java.util.Collections;
import java.util.Arrays;
import java.util.List;

import com.BV.LinearGradient.LinearGradientPackage;
import com.microsoft.codepush.react.CodePush;
import com.dieam.reactnativepushnotification.ReactNativePushNotificationPackage;
import com.facebook.reactnative.androidsdk.FBSDKPackage;

import cl.json.RNSharePackage;

public class MainActivity extends ReactActivity {
  private ReactNativePushNotificationPackage _pushNotification;
  private CallbackManager mCallbackManager;

    @Override
    protected String getJSBundleFile() {
        return CodePush.getBundleUrl();
    }

    /**
     * Returns the name of the main component registered from JavaScript.
     * This is used to schedule rendering of the component.
     */
    @Override
    protected String getMainComponentName() {
        return "F8v2";
    }

    /**
     * Returns whether dev mode should be enabled.
     * This enables e.g. the dev menu.
     */
    @Override
    protected boolean getUseDeveloperSupport() {
        return BuildConfig.DEBUG;
    }

   /**
   * A list of packages used by the app. If the app uses additional views
   * or modules besides the default ones, add more packages here.
   */
    @Override
    protected List<ReactPackage> getPackages() {
      this._pushNotification = new ReactNativePushNotificationPackage(this);
      mCallbackManager = new CallbackManager.Factory().create();

      return Arrays.<ReactPackage>asList(
        new MainReactPackage(),
        new FBSDKPackage(mCallbackManager),
        new LinearGradientPackage(),
        new RNSharePackage(),
        new CodePush("qwfkzzq7Y8cSrkiuU7aRCkIP7XYLEJ6b-AFoe", this, BuildConfig.DEBUG),
        this._pushNotification
      );
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        FacebookSdk.sdkInitialize(getApplicationContext());
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
      super.onActivityResult(requestCode, resultCode, data);
      mCallbackManager.onActivityResult(requestCode, resultCode, data);
    }

   @Override
   protected void onNewIntent (Intent intent) {
     super.onNewIntent(intent);
     _pushNotification.newIntent(intent);
   }

  @Override
  protected void onResume() {
      super.onResume();
      AppEventsLogger.activateApp(getApplicationContext());
  }

  @Override
  protected void onPause() {
      super.onPause();
      AppEventsLogger.deactivateApp(getApplicationContext());
  }

  @Override
  protected void onStop() {
      super.onStop();
      AppEventsLogger.onContextStop();
  }
}
