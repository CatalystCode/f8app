using BV.LinearGradient;
using FacebookSDK;
using ReactNative;
using ReactNative.Bridge;
using ReactNative.Modules.Core;
using ReactNative.Shell;
using Cl.Json.Share;
using ReactNative.UIManager;
using System;
using System.Collections.Generic;

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
                    new SharePackage(),
                    new F8Package(),
                    new LinearGradientPackage(),
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
