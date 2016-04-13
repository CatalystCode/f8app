using BV.LinearGradient;
using Cl.Json.Share;
using FacebookSDK;
using ReactNative;
using ReactNative.Bridge;
using CodePush.ReactNative;
using ReactNative.Modules.Core;
using ReactNative.Shell;
using ReactNative.UIManager;
using System;
using System.Collections.Generic;

namespace F8App
{
    class AppReactPage : ReactPage
    {
        private CodePushModule codePush;

        private CodePushModule CodePushInstance
        {
            get
            {
                if (codePush == null)
                {
                    codePush = new CodePushModule("deployment-key-here", this);
                }

                return codePush;
            }
        }

        public override string MainComponentName
        {
            get
            {
                return "F8v2";
            }
        }

#if BUNDLE
        public override string JavaScriptBundleFile
        {
            get
            {
                return CodePushInstance.GetJavaScriptBundleFile();
            }
        }
#endif

        public override List<IReactPackage> Packages
        {
            get
            {
                return new List<IReactPackage>
                {
                    new MainReactPackage(),
                    new F8Package(),
                    new LinearGradientPackage(),
                    new SharePackage(),
                    CodePushInstance
                };
            }
        }

        public override bool UseDeveloperSupport
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        private class F8Package : IReactPackage
        {
            private const string F8AppID = "619048868222429";
            private const string WinAppID = "s-1-15-2-635873031-2844751771-797608348-1547790894-192744704-951387951-590373624";

            public IReadOnlyList<Type> CreateJavaScriptModulesConfig()
            {
                return new List<Type>(0);
            }

            public IReadOnlyList<INativeModule> CreateNativeModules(ReactContext reactContext)
            {
                return new List<INativeModule>
                {
                    new FBSDKModule(reactContext, F8AppID, WinAppID),
                };
            }

            public IReadOnlyList<IViewManager> CreateViewManagers(ReactContext reactContext)
            {
                return new List<IViewManager>(0);
            }
        }
    }
}
