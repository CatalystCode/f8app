using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using winsdkfb;
using winsdkfb.Graph;

namespace FacebookSDK
{
    public class FBSDKModule : ReactContextNativeModuleBase
    {
        /// <summary>
        /// Settings key for Facebook App ID.
        /// </summary>
        public const string FBAppIdSettingsKey = "FBSDKModule.FBAppId";

        /// <summary>
        /// Settings key for Windows App ID configured in Facebook App.
        /// </summary>
        public const string WinAppIdSettingsKey = "FBSDKModule.WinAppId";

        private const string PublishPermissionPrefix = "publish";
        private const string ManagePermissionPrefix = "manage";

        private static readonly HashSet<string> s_otherPublishPermissions = new HashSet<string>
        {
            "ads_management",
            "create_event",
            "rsvp_event",
        };

        private static readonly FBJsonClassFactory s_jsonFactory = new FBJsonClassFactory(s => s);

        /// <summary>
        /// Instantiates a <see cref="FBSDKModule"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        public FBSDKModule(ReactContext context)
            : base(context)
        {
        }

        /// <summary>
        /// The name of the module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "FBSDK";
            }
        }

        /// <summary>
        /// Log in to Facebook with read permissions.
        /// </summary>
        /// <param name="permissionsList">The permissions list.</param>
        /// <param name="promise">A promise to resolve the request.</param>
        [ReactMethod]
        public void loginWithReadPermissions(List<string> permissionsList, IPromise promise)
        {
            ValidateReadPermissions(permissionsList);
            Login(permissionsList, promise);
        }

        /// <summary>
        /// Log in to Facebook with publish permissions.
        /// </summary>
        /// <param name="permissionsList">The permissions list.</param>
        /// <param name="promise">A promise to resolve the request.</param>
        [ReactMethod]
        public void loginWithPublishPermissions(List<string> permissionsList, IPromise promise)
        {
            ValidatePublishPermissions(permissionsList);
            Login(permissionsList, promise);
        }

        /// <summary>
        /// Log out of Facebook.
        /// </summary>
        [ReactMethod]
        public async void logOut()
        {
            var session = FBSession.ActiveSession;
            await session.LogoutAsync();
        }

        /// <summary>
        /// Make a Facebook graph request.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="version">The version.</param>
        /// <param name="method">The method.</param>
        /// <param name="promise">A promise to resolve the request.</param>
        [ReactMethod]
        public async void makeGraphRequest(
            string path,
            JObject parameters,
            string version,
            string method,
            IPromise promise)
        {
            if (version != null)
            {
                throw new NotSupportedException("Version parameter is not currently supported.");
            }

            var propertySet = new PropertySet();
            foreach (var pair in parameters)
            {
                propertySet.Add(pair.Key, pair.Value);
            }

            var request = new FBSingleValue(path, propertySet, s_jsonFactory);
            var result = await ExecuteAsync(method, request);
            if (result.Succeeded)
            {
                promise.Resolve(result.Object);
            }
            else
            {
                promise.Reject(result.ErrorInfo.Message);
            }
        }

        private static JObject CreateLoginResponse(FBSession session)
        {
            var accessToken = session.AccessTokenData;
            var user = session.User;
            return new JObject
            {
                { "tokenString", accessToken.AccessToken },
                { "permissions", new JArray(accessToken.GrantedPermissions.Values) },
                { "declinedPermissions", new JArray(accessToken.DeclinedPermissions.Values) },
                { "appID", session.FBAppId },
                { "userID", user.Id },
                { "expires", accessToken.ExpirationDate.ToUnixTimeMilliseconds() },
            };
        }

        private static IAsyncOperation<FBResult> ExecuteAsync(string method, FBSingleValue request)
        {
            switch (method.ToLowerInvariant())
            {
                case "delete":
                    return request.DeleteAsync();
                case "get":
                    return request.GetAsync();
                case "post":
                    return request.PostAsync();
                default:
                    throw new NotSupportedException($"Method '{method}' is not supported.");
            }
        }

        private static bool IsPublishPermission(string permission)
        {
            return permission != null &&
                (permission.StartsWith(PublishPermissionPrefix) ||
                    permission.StartsWith(ManagePermissionPrefix) ||
                    s_otherPublishPermissions.Contains(permission));
        }

        private static void Login(List<string> permissionsList, IPromise promise)
        {
            var fbAppId = ApplicationData.Current.LocalSettings.Values[FBAppIdSettingsKey] as string;
            if (fbAppId == null)
            {
                throw new InvalidOperationException("Facebook AppId has not been set properly in local settings.");
            }

            var winAppId = ApplicationData.Current.LocalSettings.Values[WinAppIdSettingsKey] as string;
            if (winAppId == null)
            {
                throw new InvalidOperationException("Windows AppId has not been set properly in local settings");
            }

            var session = FBSession.ActiveSession;
            session.FBAppId = fbAppId;
            session.WinAppId = winAppId;
            var permissions = new FBPermissions(permissionsList);

            RunOnDispatcher(async () =>
            {
                var result = await session.LoginAsync(permissions, SessionLoginBehavior.WebAuth);
                if (result.Succeeded)
                {
                    promise.Resolve(CreateLoginResponse(session));
                }
                else
                {
                    promise.Reject(result.ErrorInfo.Message);
                }
            });
        }

        private static async void RunOnDispatcher(DispatchedHandler action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }

        private static void ValidatePublishPermissions(List<string> permissions)
        {
            if (permissions == null)
            {
                return;
            }

            foreach (var permission in permissions)
            {
                if (!IsPublishPermission(permission))
                {
                    throw new InvalidOperationException(
                        $"Cannot pass a read permission '{permission}' to a request for publish authorization.");
                }
            }
        }

        private static void ValidateReadPermissions(List<string> permissions)
        {
            if (permissions == null)
            {
                return;
            }

            foreach (string permission in permissions)
            {
                if (IsPublishPermission(permission))
                {
                    throw new InvalidOperationException(
                        $"Cannot pass a publish or manage permission '{permission}' to a request for read authorization");
                }
            }
        }
    }
}
