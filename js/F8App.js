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
 * @providesModule F8App
 * @flow
 */

'use strict';

var React = require('React');
var AppState = require('AppState');
var BackAndroid = require('BackAndroid');
var FriendsScheduleView = require('./tabs/schedule/FriendsScheduleView');
var F8SessionDetails = require('F8SessionDetails');
var FilterScreen = require('./filter/FilterScreen');
var LoginModal = require('./login/LoginModal');
var LoginScreen = require('./login/LoginScreen');
var Navigator = require('Navigator');
var SessionsCarousel = require('./tabs/schedule/SessionsCarousel');
var SharingSettingsModal = require('./tabs/schedule/SharingSettingsModal');
var SharingSettingsScreen = require('./tabs/schedule/SharingSettingsScreen');
var RatingScreen = require('./rating/RatingScreen');
var StatusBarIOS = require('StatusBarIOS');
var StyleSheet = require('StyleSheet');
var F8Navigator = require('F8Navigator');
var View = require('View');
var StatusBar = require('StatusBar');

var codePush = require('react-native-code-push');

var {
  loadConfig,
  loadMaps,
  loadNotifications,
  loadSessions,
  loadFAQs,
  loadPages,
  loadFriendsSchedules,
  loadSurveys,
} = require('./actions');
var { updateInstallation } = require('./actions/installation');
var { connect } = require('react-redux');

var version = require('./version.js');

var F8App = React.createClass({
  componentDidMount: function() {
    AppState.addEventListener('change', this.handleAppStateChange);

    // TODO: Make this list smaller, we basically download the whole internet
    this.props.dispatch(loadNotifications());
    this.props.dispatch(loadMaps());
    this.props.dispatch(loadConfig());
    this.props.dispatch(loadFAQs());
    this.props.dispatch(loadPages());
    this.props.dispatch(loadSessions());
    this.props.dispatch(loadFriendsSchedules());
    this.props.dispatch(loadSurveys());

    updateInstallation(version);
    
    codePush.sync({
      installMode: codePush.InstallMode.IMMEDIATE,
      updateDialog: true
    });
  },

  componentWillUnmount: function() {
    AppState.removeEventListener('change', this.handleAppStateChange);
  },

  handleAppStateChange: function(appState) {
    if (appState === 'active') {
      this.props.dispatch(loadSessions());
      this.props.dispatch(loadNotifications());
      this.props.dispatch(loadSurveys());
    }
  },

  render: function() {
    if (!this.props.isLoggedIn) {
      return <LoginScreen />;
    }
    return (
      <View style={styles.container}>
        <StatusBar
          translucent={true}
          backgroundColor="rgba(0, 0, 0, 0.2)"
          barStyle="light-content"
         />
        <F8Navigator />
      </View>
    );
  },

});

var styles = StyleSheet.create({
  container: {
    flex: 1,
  },
});

function select(store) {
  return {
    isLoggedIn: store.user.isLoggedIn || store.user.hasSkippedLogin,
  };
}

module.exports = connect(select)(F8App);
