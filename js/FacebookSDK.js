/**
 * Copyright 2014 Facebook, Inc.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to
 * use, copy, modify, and distribute this software in source code or binary
 * form for use in connection with the web services and APIs provided by
 * Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use
 * of this software is subject to the Facebook Developer Principles and
 * Policies [http://developers.facebook.com/policy/]. This copyright notice
 * shall be included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE
 *
 * This module implements a part of Facebook JavaScript SDK
 * https://developers.facebook.com/docs/javascript/reference/v2.2
 *
 * @flow
 * @providesModule FacebookSDK
 */
'use strict';

var {FBSDKLoginManager} = require('react-native-fbsdklogin');
var {FBSDKAccessToken, FBSDKGraphRequest} = require('react-native-fbsdkcore');
const FBSDK = require('react-native-fbsdk2');
const Platform = require('Platform');

var emptyFunction = () => {};
var mapObject = require('fbjs/lib/mapObject');


type AuthResponse = {
  userID: string;
  accessToken: string;
  expiresIn: number;
};
type LoginOptions = { scope: string };
type LoginCallback = (result: {authResponse?: AuthResponse, error?: Error}) => void;

var _authResponse: ?AuthResponse = null;

function handleIOSLoginCallback(callback: LoginCallback, error, result) {
  if (error) {
    callback({error});
    return;
  }
  if (!result || result.isCancelled) {
    callback({error: new Error('Canceled by user')});
    return;
  }
  FBSDKAccessToken.getCurrentAccessToken(
    (token) => {
      if (!token) {
        callback({error: new Error('No access token')});
        return;
      }
      _authResponse = {
        userID: token.userID,
        accessToken: token.tokenString,
        expiresIn: Math.round((token.getExpirationDate() - Date.now()) / 1000),
      };
      callback({authResponse: _authResponse});
    }
  );
}

function handleAndroidLoginCallback(callback: LoginCallback, error, result) {
  if (error) {
    callback({error});
    return;
  }
  if (!result || result.isCancelled) {
    callback({error: new Error('Canceled by user')});
    return;
  }
  _authResponse = {
    userID: result.userID,
    accessToken: result.tokenString,
    expiresIn: Math.round((result.expires - Date.now()) / 1000),
  };
  callback({authResponse: _authResponse});
}

function handleWindowsLoginCallback(callback: LoginCallback, error, result) {
  if (error) {
    callback({error});
    return;
  }
  if (!result || result.isCancelled) {
    callback({error: new Error('Canceled by user')});
    return;
  }
  _authResponse = {
    userID: result.userID,
    accessToken: result.tokenString,
    expiresIn: Math.round((result.expires - Date.now()) / 1000),
  };
  callback({authResponse: _authResponse});
}

var FacebookSDK = {
  init: function() {
    // This is needed by Parse
    window.FB = FacebookSDK;
  },

  login: function(callback: LoginCallback, options: LoginOptions) {
    const scope = options.scope || 'public_profile';
    const permissions = scope.split(',');
    if (Platform.OS === 'ios') {
      FBSDKLoginManager.logInWithReadPermissions(
        permissions,
        (error, result) => handleIOSLoginCallback(callback, error, result));
    } else if (Platform.OS === 'android') {
      FBSDK.loginWithReadPermissions(permissions).then(
        (result) => handleAndroidLoginCallback(callback, null, result),
        (error) => handleAndroidLoginCallback(callback, error, null)
      );
    } else if (Platform.OS === 'windows') {
      FBSDK.loginWithReadPermissions(permissions).then(
        (result) => handleWindowsLoginCallback(callback, null, result),
        (error) => handleWindowsLoginCallback(callback, error, null),        
      )
    }
  },

  getAuthResponse: function(): ?AuthResponse {
    return _authResponse;
  },

  logout: function() {
    if (Platform.OS === 'ios') {
      FBSDKLoginManager.logOut();
    } else {
      FBSDK.logOut();
    }
  },

  /**
   * Make a API call to Graph server. This is the **real** RESTful API.
   *
   * Except the path, all arguments to this function are optional. So any of
   * these are valid:
   *
   *   FB.api('/me') // throw away the response
   *   FB.api('/me', function(r) { console.log(r) })
   *   FB.api('/me', { fields: 'email' }); // throw away response
   *   FB.api('/me', { fields: 'email' }, function(r) { console.log(r) });
   *   FB.api('/12345678', 'delete', function(r) { console.log(r) });
   *   FB.api(
   *     '/me/feed',
   *     'post',
   *     { body: 'hi there' },
   *     function(r) { console.log(r) }
   *   );
   *
   * param path   {String}   the url path
   * param method {String}   the http method
   * param params {Object}   the parameters for the query
   * param cb     {Function} the callback function to handle the response
   */
  api: function(path: string, ...args: Array<mixed>) {
    var argByType = {};
    args.forEach((arg) => { argByType[typeof arg] = arg; });

    var method = argByType['string'] || 'get';
    var params = argByType['object'] || {};
    var callback = argByType['function'] || emptyFunction;

    // FBSDKGraphRequest requires all params to be in {string: 'abc'}
    // or {uri: 'xyz'} format
    var typedParams = mapObject(params, (value) => ({string: value}));

    if (Platform.OS === 'ios') {
      var request = new FBSDKGraphRequest(
        (error, result) => {
          var data = error ? {error} : result;
          callback(data);
        },
        path,
        typedParams,
        null,
        null,
        method
      );
      request.start();
    } else if (Platform.OS === 'android') {
      FBSDK.makeGraphRequest(path, typedParams, null, method.toUpperCase()).then(
        (result) => callback(JSON.parse(result)),
        (error) => callback({error})
      );
    } else if (Platform.OS === 'windows') {
      FBSDK.makeGraphRequest(path, params, null, method.toUpperCase()).then(
        (result) => callback(JSON.parse(result)),
        (error) => callback({error})          
      );
    }
  }
};

module.exports = FacebookSDK;
