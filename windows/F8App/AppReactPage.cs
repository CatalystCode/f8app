using BV.LinearGradient;
using Cl.Json.Share;
using FacebookSDK;
using ReactNative;
using ReactNative.Bridge;
using ReactNative.Modules.Core;
using ReactNative.Shell;
using ReactNative.UIManager;
using System;
using System.Collections.Generic;
using Share;

namespace F8App
{
    class AppReactPage : ReactPage
    {
        public override string MainComponentName
        {
            get
            {
                return "F8v2";
            }
        }

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
                };
            }
        }

        public override bool UseDeveloperSupport
        {
            get
            {
                return true;
            }
        }

        private class F8Package : IReactPackage
        {
            public IReadOnlyList<Type> CreateJavaScriptModulesConfig()
            {
                return new List<Type>(0);
            }

            public IReadOnlyList<INativeModule> CreateNativeModules(ReactContext reactContext)
            {
                return new List<INativeModule>
                {
                    new FBSDKModule(reactContext),
                };
            }

            public IReadOnlyList<IViewManager> CreateViewManagers(ReactContext reactContext)
            {
                return new List<IViewManager>(0);
            }
        }
    }
}
