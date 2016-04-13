using ReactNative.UIManager;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BV.LinearGradient
{
    /// <summary>
    /// View manager for linear gradient view parent control.
    /// </summary>
    public class LinearGradientViewManager : BorderedCanvasManager<Canvas>
    {
        private readonly IDictionary<Border, LinearGradientSettings> _settings =
            new Dictionary<Border, LinearGradientSettings>();

        /// <summary>
        /// The name of the view manager.
        /// </summary>
        public override string Name
        {
            get
            {
                return "BVLinearGradient";
            }
        }

        /// <summary>
        /// Set the gradient direction line start point.
        /// </summary>
        /// <param name="view">The view instance.</param>
        /// <param name="start">The</param>
        [ReactProperty("start")]
        public void SetStart(Border view, double[] start)
        {
            var settings = EnsureSettings(view);
            settings.Start = start;
        }

        /// <summary>
        /// Set the gradient direction line end point.
        /// </summary>
        /// <param name="view">The view instance.</param>
        /// <param name="end">The</param>
        [ReactProperty("end")]
        public void SetEnd(Border view, double[] end)
        {
            var settings = EnsureSettings(view);
            settings.End = end;
        }

        /// <summary>
        /// Set the gradient colors.
        /// </summary>
        /// <param name="view">The view instance.</param>
        /// <param name="colors">The</param>
        [ReactProperty("colors")]
        public void SetColors(Border view, uint[] colors)
        {
            var settings = EnsureSettings(view);
            settings.Colors = colors;
        }

        /// <summary>
        /// Set the offset locations for the gradient stops.
        /// </summary>
        /// <param name="view">The view instance.</param>
        /// <param name="locations">The</param>
        [ReactProperty("locations")]
        public void SetLocations(Border view, double[] locations)
        {
            var settings = EnsureSettings(view);
            settings.Locations = locations;
        }

        /// <summary>
        /// Called when view is detached from view hierarchy and allows for 
        /// additional cleanup by the view manager.
        /// </summary>
        /// <param name="reactContext">The react context.</param>
        /// <param name="view">The view.</param>
        /// <remarks>
        /// Derived classes do not need to call this base method.
        /// </remarks>
        public override void OnDropViewInstance(ThemedReactContext reactContext, Border view)
        {
            _settings.Remove(view);
        }

        /// <summary>
        /// Creates a new view instance of type <typeparamref name="TFrameworkElement"/>.
        /// </summary>
        /// <param name="reactContext">The react context.</param>
        /// <returns>The view instance.</returns>
        protected override Canvas CreateInnerElement(ThemedReactContext reactContext)
        {
            return new Canvas();
        }

        /// <summary>
        /// Callback that will be triggered after all properties are updated in
        /// the current update transation (all <see cref="ReactPropertyAttribute"/> handlers
        /// for properties updated in the current transaction have been called).
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnAfterUpdateTransaction(Border view)
        {
            var settings = EnsureSettings(view);
            var brush = settings.CreateBrush();
            view.Background = brush;
        }

        private LinearGradientSettings EnsureSettings(Border view)
        {
            var result = default(LinearGradientSettings);
            if (!_settings.TryGetValue(view, out result))
            {
                result = new LinearGradientSettings();
                _settings.Add(view, result);
            }

            return result;
        }

        class LinearGradientSettings
        {
            public double[] Start { get; set; }

            public double[] End { get; set; }

            public uint[] Colors { get; set; }

            public double[] Locations { get; set; }

            public LinearGradientBrush CreateBrush()
            {
                if (Colors?.Length > 0)
                {
                    var delta = 1.0;
                    var useDelta = true;
                    var stopCollection = new GradientStopCollection();

                    if (Locations?.Length == Colors.Length)
                    {
                        useDelta = false;
                    }
                    else if (Colors.Length > 1)
                    {
                        delta = 1.0 / (Colors.Length - 1);
                    }

                    for (var i = 0; i < Colors.Length; i++)
                    {
                        var stop = new GradientStop();
                        stop.Color = Parse(Colors[i]);

                        if (useDelta)
                        {
                            stop.Offset = i * delta;
                        }
                        else
                        {
                            stop.Offset = Locations[i];
                        }

                        stopCollection.Add(stop);
                    }

                    var angle = 90.0;

                    if (Start?.Length == 2 && End?.Length == 2)
                    {
                        var x = End[0] - Start[0];
                        var y = End[1] - Start[1];

                        if (x != 0)
                        {
                            angle = 90.0 - Math.Atan(y / x);
                        }
                        else
                        {
                            angle = 0;
                        }
                    }

                    return new LinearGradientBrush(stopCollection, angle);
                }

                return null;
            }

            private static Color Parse(uint value)
            {
                var color = value;
                var b = (byte)color;
                color >>= 8;
                var g = (byte)color;
                color >>= 8;
                var r = (byte)color;
                color >>= 8;
                var a = (byte)color;
                return Color.FromArgb(a, r, g, b);
            }
        }
    }
}
