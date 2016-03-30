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
 * @flow
 * @providesModule F8BigHeader
 */

'use strict';

var Animated = require('Animated');
var TouchableOpacity = require('TouchableOpacity');
var React = require('React');
var StyleSheet = require('StyleSheet');
var { Text } = require('F8Text');
var View = require('View');
var Image = require('Image');
var Dimensions = require('Dimensions');
var LinearGradient = require('react-native-linear-gradient');

const HEIGHT = Dimensions.get('window').height > 600
  ? 200
  : 150;

class F8BigHeader extends React.Component {
  props: {
    title: string;
    image: ?ReactElement;
    showCollapsedTitle: boolean,
    colors: Array<string>;
    offset: any;
    curve: number;
  };

  render() {
    const contentOpacity = this.props.offset.interpolate({
      inputRange: [0, HEIGHT],
      outputRange: [1, 0],
    });
    const showWhenCollapsed = this.props.offset.interpolate({
      inputRange: [HEIGHT - 1, HEIGHT],
      outputRange: [0, 1],
      extrapolate: 'clamp',
    });
    const hideWhenCollapsed = this.props.offset.interpolate({
      inputRange: [HEIGHT - 40, HEIGHT],
      outputRange: [1, 0],
      extrapolate: 'clamp',
    });
    const contentShift = this.props.offset.interpolate({
      inputRange: [0, HEIGHT],
      outputRange: [-32, -(HEIGHT / 2) - 32],
      extrapolate: 'clamp',
    });
    const backgroundShift = this.props.offset.interpolate({
      inputRange: [0, HEIGHT],
      outputRange: [-32, -HEIGHT - 32],
      extrapolate: 'clamp',
    });
    const backgroundZoom = this.props.offset.interpolate({
      inputRange: [-HEIGHT / 2, 0],
      outputRange: [1.2, 1],
      extrapolateRight: 'clamp',
    });
    const content = this.props.image || <Text style={styles.contentText}>{this.props.title}</Text>;
    const expandedTitle = this.props.image ? this.props.title : '';
    const collapsedTitle = this.props.showCollapsedTitle ? this.props.title : '';

    return (
      <LinearGradient
        colors={this.props.colors}
        locations={[0, 0.7]}
        style={styles.header}>
        <Animated.Image
          source={this.props.curve}
          style={[styles.backgroundImage, {
            transform: [{translateY: backgroundShift}, {scale: backgroundZoom}],
          }]}
        />
        <Animated.View style={[styles.expanded, {
          opacity: contentOpacity,
          transform: [{translateY: contentShift}],
        }]}>
          {content}
        </Animated.View>
        <Animated.View style={[styles.title, {opacity: showWhenCollapsed}]}>
          <Text style={styles.titleText}>
            {collapsedTitle}
          </Text>
        </Animated.View>
        <Animated.View style={[styles.title, {opacity: hideWhenCollapsed}]}>
          <Text style={styles.titleText}>
            {expandedTitle}
          </Text>
        </Animated.View>
      </LinearGradient>
    );
  }
}

F8BigHeader.HEIGHT = HEIGHT;

var STATUS_BAR_HEIGHT = 20;
var HEADER_HEIGHT = HEIGHT + 156;

var styles = StyleSheet.create({
  header: {
    position: 'absolute',
    left: 0,
    top: 0,
    right: 0,
    height: HEADER_HEIGHT,
    backgroundColor: 'transparent',
    alignItems: 'center',
  },
  backgroundImage: {
    alignItems: 'center',
    justifyContent: 'center',
  },
  titleText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: 20,
  },
  contentText: {
    backgroundColor: 'transparent',
    color: 'white',
    fontSize: 42,
  },
  expanded: {
    position: 'absolute',
    left: 0,
    top: 0,
    right: 0,
    height: HEADER_HEIGHT,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: 'transparent',
  },
  title: {
    position: 'absolute',
    left: 0,
    top: STATUS_BAR_HEIGHT,
    right: 0,
    height: 44,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: 'transparent',
  },
});


module.exports = F8BigHeader;
