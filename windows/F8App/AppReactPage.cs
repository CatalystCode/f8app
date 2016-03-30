using System;
using System.Collections.Generic;
using ReactNative;
using ReactNative.Modules.Core;
using ReactNative.Shell;
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
    }
}
