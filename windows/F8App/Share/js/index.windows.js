/**
 * @providesModule RNShare
 * @flow
 */
'use strict';

var { NativeModules } = require('react-native');
var NativeRNShare = NativeModules.RNShare;

var RNShare = {
  open: function(options) {
  	NativeRNShare.open(options);
  }
};

module.exports = RNShare;
