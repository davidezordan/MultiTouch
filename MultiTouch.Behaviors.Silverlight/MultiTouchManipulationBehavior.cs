// ****************************************************************************
//Microsoft Surface Manipulations and Inertia Sample for Microsoft Silverlight
//http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=4b281bde-9b01-4890-b3d4-b3b45ca2c2e4
//The contents of this Sample download (Microsoft Surface Silverlight Manipulations and Interia Sample.zip) are covered by the Microsoft Surface SDK 1.0 SP1 license agreement, with any additional restrictions noted here.  The purpose of this download is for educational use only and is made available AS-IS with no support.
//This package contains APIs for using manipulation and inertia in Silverlight.
//Multi-touch support in Microsoft Windows 7 allows applications to blur the lines between computers and the real world.  Touch-optimized applications entice users to touch the objects on the screen, drag them across the screen, rotate and resize them, and flick them across the screen using their fingers.
//Manipulations and Inertia processor classes allow graphical user interface (GUI) components to move in a natural and intuitive way. Manipulations enable users to move, rotate, and resize components by using their fingers. Inertia enables users to move components by applying forces on the components, such as flicking the component across the screen.
//Contents:
//Binary
//    \System.Windows.Input.Manipulations.dll
//This Silverlight assembly contains APIs which applications and controls can use to implement multi-touch manipulations & inertia gesture functionality.  This is equivalent to the System.Windows.Input.Manipulation assembly included in the Microsoft .NET Framework 4.0 and is provided for educational purposes only (this sample version of this assembly is not redistributable for commercial environments).  For API documentation, please refer to http://msdn.microsoft.com/en-us/library/system.windows.input.manipulations(VS.100).aspx
//Sample source
//    \Sample\Code
//This folder contains a Visual Studio project demonstrates the usage of the ManipulationProcessor2D and InertiaProcessor2D APIs to create UI elements which can be resized, rotated, moved, and flicked around the screen.
// ****************************************************************************

// ****************************************************************************
// <copyright file="MultiTouchManipulationBehavior.cs" company="Davide Zordan">
// Copyright © Davide Zordan 2010-2014
// </copyright>
// ****************************************************************************
// <author>Davide Zordan</author>
// <email>info@davidezordan.net</email>
// <date>05.08.2011</date>
// <project>MultiTouch.Behaviors.Silverlight4</project>
// <web>http://multitouch.codeplex.com/</web>
// <license>
// See http://multitouch.codeplex.com/license.
// </license>
// ****************************************************************************
// SAMPLE CODE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE, ARE DISCLAIMED.  IN NO EVENT SHALL ESRI OR CONTRIBUTORS
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) SUSTAINED BY YOU OR A THIRD PARTY, HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT; STRICT LIABILITY; OR TORT ARISING
// IN ANY WAY OUT OF THE USE OF THIS SAMPLE CODE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE TO THE FULL EXTENT ALLOWED BY APPLICABLE LAW.

using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System;
using System.Windows.Input.Manipulations;
using MultiTouch.ManipulationLib.Silverlight;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media;

//Used by Touch Markers in Debug mode
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

#if DEBUG
using System.Diagnostics;
#endif


namespace MultiTouch.Behaviors.Silverlight
{
    /// <summary>
    /// Implements Multi-Touch Manipulation
    /// </summary>
    internal class MultiTouchManipulationBehavior : Behavior<FrameworkElement>, IDisposable
    {
        /// <summary>
        /// Reset the Touch handlers for the AssociatedObject
        /// </summary>
        private void ResetAssociatedObject(FrameworkElement associatedObject)
        {
            if (associatedObject == null) return;
            associatedObject.MouseLeftButtonUp -= OnMouseUp;
            associatedObject.MouseLeftButtonDown -= OnMouseDown;
            associatedObject.MouseMove -= OnMouseMove;
            associatedObject.LostMouseCapture -= OnLostMouseCapture;
            associatedObject.Loaded -= AssociatedObjectLoaded;
            TouchHelper.RemoveHandlers(associatedObject);
            // Better to not disable the TouchHelper here otherwise touch events will be disabled for all other elements using the Behavior
            // TouchHelper.EnableInput(false);
        }
        
        /// <summary>
        /// Initialize Touch handlers for the AssociatedObject
        /// </summary>
        private void InitializeAssociatedObject(FrameworkElement associatedObject)
        {
            if (associatedObject == null) return;
            associatedObject.MouseLeftButtonUp += OnMouseUp;
            associatedObject.MouseLeftButtonDown += OnMouseDown;
            associatedObject.MouseMove += OnMouseMove;
            associatedObject.LostMouseCapture += OnLostMouseCapture;

            TouchHelper.AddHandlers(associatedObject, 
                new TouchHandlers
                    {
                        TouchDown = OnTouchDown,
                        CapturedTouchReported = OnCapturedTouchReported,
                        CapturedTouchUp = OnTouchUp
                    });

            TouchHelper.EnableInput(true);
            associatedObject.Loaded += AssociatedObjectLoaded;
        }

        /// <summary>
        /// Initialize the behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            //Initialise default values for public properties
            IsConstrainedToParentBounds = true;
            IsRotateEnabled = true;
            IsTranslateEnabled = true;
            IsScaleEnabled = true;
            IsPivotEnabled = true;
            AreFingersVisible = true;
            IsInertiaEnabled = true;
            AreManipulationsEnabled = true;
            MinimumScaleRadius = 60;
            MaximumScaleRadius = 240;
            IgnoredTypes = null;

            _manipulationProcessor = new ManipulationProcessor2D(SupportedManipulations);
            _manipulationProcessor.Started += OnManipulationStarted;
            _manipulationProcessor.Delta += OnManipulationDelta;
            _manipulationProcessor.Completed += OnManipulationCompleted;

            _inertiaProcessor = new InertiaProcessor2D
            {
                TranslationBehavior = { DesiredDeceleration = Deceleration },
                RotationBehavior = { DesiredDeceleration = AngularDeceleration },
                ExpansionBehavior = { DesiredDeceleration = ExpansionDeceleration }
            };

            _inertiaProcessor.Delta += OnManipulationDelta;
            _inertiaProcessor.Completed += OnInertiaCompleted;

            _inertiaTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            _inertiaTimer.Tick += OnTimerTick;

            var centerX = (double)AssociatedObject.ActualWidth / 2 + (double)AssociatedObject.GetValue(Canvas.LeftProperty);
            var centerY = (double)AssociatedObject.ActualHeight / 2 + (double)AssociatedObject.GetValue(Canvas.TopProperty);

            AssociatedObject.RenderTransformOrigin = new Point(centerX, centerY);

            Move(new Point(centerX, centerY), 0, 100);

            InitializeAssociatedObject(AssociatedObject);
        }

        /// <summary>
        /// Initialize the TouchHelper
        /// </summary>
        void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            TouchHelper.SetRootElement(TouchHelper.GetRootElement(AssociatedObject));
        }

        /// <summary>
        /// Occurs when detaching the behavior
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            _manipulationProcessor.Started -= OnManipulationStarted;
            _manipulationProcessor.Delta -= OnManipulationDelta;
            _manipulationProcessor.Completed -= OnManipulationCompleted;
            _inertiaProcessor.Delta -= OnManipulationDelta;
            _inertiaProcessor.Completed -= OnInertiaCompleted;
            _inertiaTimer.Tick -= OnTimerTick;

            ResetAssociatedObject(AssociatedObject);
        }

        #region Private fields

        /// <summary>
        /// Gets the center of the item, in container coordinates.
        /// </summary>
        private Point _center;

        /// <summary>
        /// Gets or sets the orientation of the object, in degrees.
        /// </summary>
        private double _orientation;

        /// <summary>
        /// Gets or sets the radius of the object, in pixels.
        /// </summary>
        private double _radius;

        /// <summary>
        /// Touch markers
        /// </summary>
        private readonly Dictionary<int, Popup> _touchPointsMarkers = new Dictionary<int, Popup>();

        /// <summary>
        /// Point in Debug Mode
        /// </summary>
        private const double EllipseWidth = 75;

        // default dpi
        private const float DefaultDpi = 96.0f;

        // deceleration: inches/second squared 
        private const float Deceleration = 10.0f * DefaultDpi / (1000.0f * 1000.0f);
        private const float ExpansionDeceleration = 16.0f * DefaultDpi / (1000.0f * 1000.0f);

        // angular deceleration: degrees/second squared
        private const float AngularDeceleration = 270.0f / 180.0f * (float)Math.PI / (1000.0f * 1000.0f);

        // minimum/maximum flick velocities
        private const float MinimumFlickVelocity = 2.0f * DefaultDpi / 1000.0f;                      // =2 inches per sec
        private const float MinimumAngularFlickVelocity = 45.0f / 180.0f * (float)Math.PI / 1000.0f; // =45 degrees per sec
        private const float MinimumExpansionFlickVelocity = 2.0f * DefaultDpi / 1000.0f;             // =2 inches per sec
        private const float MaximumFlickVelocityFactor = 15f;

        // indicates if the mouse is captured or not
        private bool _isMouseCaptured;

        // manipulation and inertia processors
        private ManipulationProcessor2D _manipulationProcessor;
        private InertiaProcessor2D _inertiaProcessor;
        private DispatcherTimer _inertiaTimer;

        #endregion

        #region Mouse handlers
        /// <summary>
        /// Here when the mouse goes down on the item.
        /// </summary>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // ignore mouse if there is at least on
            if (!TouchHelper.AreAnyTouches && AssociatedObject.CaptureMouse())
            {
                _isMouseCaptured = true;
                ProcessMouse(e);
                BringToFront();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Here when the mouse goes up.
        /// </summary>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
                e.Handled = true;
            }
        }


        /// <summary>
        /// Here when the mouse moves.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseCaptured)
            {
                if (TouchHelper.AreAnyTouches)
                {
                    // ignore mouse if there are any touches
                    AssociatedObject.ReleaseMouseCapture();
                }
                else
                {
                    ProcessMouse(e);
                }
            }
        }

        /// <summary>
        /// Here when we've lost mouse capture.
        /// </summary>
        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (_isMouseCaptured)
            {
                _manipulationProcessor.ProcessManipulators(Timestamp, null);
                _isMouseCaptured = false;
            }
        }

        /// <summary>
        /// Process a mouse event. Note: mouse and touches at the same time are not supported.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessMouse(MouseEventArgs e)
        {
            var parent = this.AssociatedObject.Parent as UIElement;
            if (parent == null)
            {
                return;
            }

            var position = e.GetPosition(parent);
            var manipulators = new List<Manipulator2D>
                                   {
                                       new Manipulator2D(
                                           0,
                                           (float) (position.X),
                                           (float) (position.Y))
                                   };

            _manipulationProcessor.ProcessManipulators(
               Timestamp,
               manipulators);
        }
        #endregion

        #region Touch handlers

        /// <summary>
        /// Verify if the type of the ancestor is contained in the ignored types
        /// </summary>
        private static bool HasAncestor(DependencyObject child, IEnumerable<Type> types)
        {
            var parent = VisualTreeHelper.GetParent(child) as UIElement;
            if (parent == null) { return false; }
            if (types.Any(t => parent.GetType() == t))
            {
                return true;
            }
            return parent != Application.Current.RootVisual && HasAncestor(parent, types);
        }

        /// <summary>
        /// Occurs when a Touch Point is pressed
        /// </summary>
        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            if (this.IgnoredTypes != null)
            {
                // Ignore Touch for the controls contained in the IgnoredTypes property
                var element = e.TouchPoint.TouchDevice.DirectlyOver;
                if (HasAncestor(element, this.IgnoredTypes)) { return; }
            }

            if (e.TouchPoint.TouchDevice.Capture(AssociatedObject))
            {
                BringToFront();
            }

            //Shows the touch point marker if the "AreFingersVisible" property is enabled
            if (AreFingersVisible)
            {
                var ellipse = new Ellipse
                                  {
                                      Fill = new SolidColorBrush(Colors.Blue),
                                      Opacity = .5,
                                      Width = EllipseWidth,
                                      Height = EllipseWidth
                                  };
                var ellipseContainer = new Popup
                                           {
                                               Child = ellipse,
                                               HorizontalOffset = e.TouchPoint.Position.X - (EllipseWidth/2),
                                               VerticalOffset = e.TouchPoint.Position.Y - (EllipseWidth/2),
                                               IsOpen = true
                                           };
                _touchPointsMarkers[e.TouchPoint.TouchDevice.Id] = ellipseContainer;
            }
#if DEBUG
            Debug.WriteLine("TouchPoint Down: Id {0} at ({1})", e.TouchPoint.TouchDevice.Id, e.TouchPoint.Position);
#endif
        }

        /// <summary>
        /// Occurs when a touchPoint is Released
        /// </summary>
        private void OnTouchUp(object sender, TouchEventArgs e)
        {
#if WINDOWS_PHONE
            //Workaround for the Touch.FrameReported issue in WP7 not releasing Touch Points
            TouchHelper.ResetTouchPoints();
#endif

            if (AreFingersVisible)
            {
#if WINDOWS_PHONE
                //Workaround: Remove all the Markers in the WP7 version
                if (_touchPointsMarkers!=null)
                    foreach (var touchPointsMarker in
                        _touchPointsMarkers
                        .Where(touchPointsMarker => touchPointsMarker.Value != null))
                    {
                        touchPointsMarker.Value.IsOpen = false;
                    }
#else
                //Hide the TouchPoint
                if (_touchPointsMarkers.ContainsKey(e.TouchPoint.TouchDevice.Id))
                {
                    _touchPointsMarkers[e.TouchPoint.TouchDevice.Id].IsOpen = false;
                    _touchPointsMarkers[e.TouchPoint.TouchDevice.Id] = null;
                    _touchPointsMarkers.Remove(e.TouchPoint.TouchDevice.Id);
                }
#endif
            }
#if DEBUG
            Debug.WriteLine("TouchPoint Up: Id {0} at ({1})", e.TouchPoint.TouchDevice.Id, e.TouchPoint.Position);
#endif
        }

        /// <summary>
        /// Occurs when Touch points are reported: handles manipulations
        /// </summary>
        private void OnCapturedTouchReported(object sender, TouchReportedEventArgs e)
        {
            var parent = AssociatedObject.Parent as UIElement;
            if (parent == null) return;

            //Find the root element
            var root = TouchHelper.GetRootElement(parent);
            if (root == null) return;

            //Multi-Page support: verify if the collection of Touch points is null
            var touchPoints = e.TouchPoints;
            List<Manipulator2D> manipulators = null;

            if (touchPoints.FirstOrDefault() != null)
            {
                // get transformation to convert positions to the parent's coordinate system
                var transform = root.TransformToVisual(parent);
                foreach (var touchPoint in touchPoints)
                {
                    var position = touchPoint.Position;

                    // convert to the parent's coordinate system
                    position = transform.Transform(position);

                    // create a manipulator
                    var manipulator = new Manipulator2D(
                        touchPoint.TouchDevice.Id,
                        (float) (position.X),
                        (float) (position.Y));

                    if (manipulators == null)
                    {
                        // lazy initialization
                        manipulators = new List<Manipulator2D>();
                    }
                    manipulators.Add(manipulator);

                    //Change the visualization of the touchPoint
                    if (_touchPointsMarkers.ContainsKey(touchPoint.TouchDevice.Id))
                    {
                        if (AreFingersVisible)
                        {
                            _touchPointsMarkers[touchPoint.TouchDevice.Id].HorizontalOffset =
                                touchPoint.Position.X - (EllipseWidth/2);
                            _touchPointsMarkers[touchPoint.TouchDevice.Id].VerticalOffset =
                                touchPoint.Position.Y - (EllipseWidth/2);
                        }
#if DEBUG
                        Debug.WriteLine("TouchPoint Reported: Id {0} at ({1} - Total Touch Points: {2})",
                                        touchPoint.TouchDevice.Id,
                                        touchPoint.Position, touchPoints.Count());
#endif
                    }
                }
            }

            // process manipulations
            _manipulationProcessor.ProcessManipulators(Timestamp,manipulators);
        }
        #endregion

        #region Item properties

        /// <summary>
        /// Gets or sets the AreManipulationsEnabled: if false all manipulations are disabled
        /// </summary>
        public bool AreManipulationsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the IgnoredTypes
        /// </summary>
        public Type[] IgnoredTypes { get; set; }

        /// <summary>
        /// Gets or sets the IsConstrainedToParentBounds: if true the translation manipulation is constrained to the Parent UI Element
        /// </summary>
        public bool IsConstrainedToParentBounds { get; set; }

        /// <summary>
        /// Gets or sets the IsInertiaEnabled: if true Inertia is enabled
        /// </summary>
        public bool IsInertiaEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Minimum scale radius for scaling
        /// </summary>
        public double MinimumScaleRadius { get; set; }

        /// <summary>
        /// Gets or sets the Maximum scale radius for scaling
        /// </summary>
        public double MaximumScaleRadius { get; set; }

        /// <summary>
        /// Gets or sets the IsTranslateEnabled property: if true translation is enabled
        /// </summary>
        public bool IsTranslateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the AreFingersVisible property: if true the fingers markers are shown
        /// </summary>
        public bool AreFingersVisible { get; set; }

        /// <summary>
        /// Gets or sets the IsRotateEnabled property: if true rotation is enabled
        /// </summary>
        public bool IsRotateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the IsScaleEnabled property: if true scaling is enabled
        /// </summary>
        public bool IsScaleEnabled { get; set; }

        /// <summary>
        /// Gets the CenterX of the manipulation
        /// </summary>
        public double CenterX
        {
            get { return _center.X; }
        }

        /// <summary>
        /// Gets the CenterY of the manipulation
        /// </summary>
        public double CenterY
        {
            get { return _center.Y; }
        }

        /// <summary>
        /// Gets the current Scale
        /// </summary>
        public double Scale
        {
            get { return _radius; }
        }

        /// <summary>
        /// Gets the current Angle of the rotation
        /// </summary>
        public double Rotation
        {
            get { return _orientation; }
        }

        /// <summary>
        /// The Manipulation Processor
        /// </summary>
        public ManipulationProcessor2D ManipulationProcessor
        {
            get
            {
                return this._manipulationProcessor;
            }
        }

        /// <summary>
        /// The Inertia processor
        /// </summary>
        public InertiaProcessor2D InertiaProcessor
        {
            get
            {
                return this._inertiaProcessor;
            }
        }

        /// <summary>
        /// Gets or sets whether the pivot is active.
        /// </summary>
        public bool IsPivotEnabled
        {
            get
            {
                return _manipulationProcessor.Pivot != null;
            }
            set
            {
                UpdatePivot(value);
            }
        }


        /// <summary>
        /// Gets the current timestamp.
        /// </summary>
        private static long Timestamp
        {
            get
            {
                // The question of what tick source to use is a difficult
                // one in general, but for purposes of this test app,
                // DateTime ticks are good enough.
                return DateTime.UtcNow.Ticks;
            }
        }
        #endregion

        #region Manipulation handlers
        /// <summary>
        /// Stops inertia.
        /// </summary>
        private void StopInertia()
        {
            if (_inertiaProcessor.IsRunning) _inertiaProcessor.Complete(Timestamp);

            //  always stop the timer
            _inertiaTimer.Stop();
        }

        /// <summary>
        /// Here when manipulation starts.
        /// </summary>
        private void OnManipulationStarted(object sender, Manipulation2DStartedEventArgs e)
        {
            StopInertia();
        }

        /// <summary>
        /// Here when manipulation gives a delta.
        /// </summary>
        private void OnManipulationDelta(object sender, Manipulation2DDeltaEventArgs e)
        {
            if (!this.AreManipulationsEnabled)
            {
                return;
            }

            Move(new Point(e.OriginX, e.OriginY),
                new Vector(e.Delta.TranslationX, e.Delta.TranslationY),
                e.Delta.Rotation,
                e.Delta.ScaleX);
        }

        /// <summary>
        /// Here when manipulation completes.
        /// </summary>
        private void OnManipulationCompleted(object sender, Manipulation2DCompletedEventArgs e)
        {
            // Get the inital inertia values
            var initialVelocity = new Vector(e.Velocities.LinearVelocityX, e.Velocities.LinearVelocityY);
            float angularVelocity = e.Velocities.AngularVelocity;
            float expansionVelocity = e.Velocities.ExpansionVelocityX;

            bool startFlick = false;

            // Rotate and scale around the center of the item
            _inertiaProcessor.InitialOriginX = (float)_center.X;
            _inertiaProcessor.InitialOriginY = (float)_center.Y;

            // set initial velocity if translate flicks are allowed
            double velocityLengthSquared = initialVelocity.LengthSquared;
            if (IsTranslateEnabled && velocityLengthSquared > MinimumFlickVelocity * MinimumFlickVelocity)
            {
                const double maximumLengthSquared = MaximumFlickVelocityFactor * MinimumFlickVelocity * MinimumFlickVelocity;
                if (velocityLengthSquared > maximumLengthSquared)
                {
                    initialVelocity = Math.Sqrt(maximumLengthSquared / velocityLengthSquared) * initialVelocity;
                }

                startFlick = IsInertiaEnabled;
                _inertiaProcessor.TranslationBehavior.InitialVelocityX = (float)initialVelocity.X;
                _inertiaProcessor.TranslationBehavior.InitialVelocityY = (float)initialVelocity.Y;
            }
            else
            {
                _inertiaProcessor.TranslationBehavior.InitialVelocityX = 0.0f;
                _inertiaProcessor.TranslationBehavior.InitialVelocityY = 0.0f;
            }

            // set angular velocity if rotation flicks are allowed
            if (Math.Abs(angularVelocity) >= MinimumAngularFlickVelocity)
            {
                const float maximumAngularFlickVelocity = MaximumFlickVelocityFactor * MinimumAngularFlickVelocity;
                if (Math.Abs(angularVelocity) > maximumAngularFlickVelocity)
                {
                    angularVelocity = angularVelocity > 0 ? maximumAngularFlickVelocity : -maximumAngularFlickVelocity;
                }
                startFlick = IsInertiaEnabled;
                _inertiaProcessor.RotationBehavior.InitialVelocity = angularVelocity;
            }
            else
            {
                _inertiaProcessor.RotationBehavior.InitialVelocity = 0.0f;
            }

            // set expansion velocity if scale flicks are allowed
            if (IsScaleEnabled && Math.Abs(expansionVelocity) >= MinimumExpansionFlickVelocity)
            {
                const float maximumExpansionFlickVelocity = MaximumFlickVelocityFactor * MinimumExpansionFlickVelocity;
                if (Math.Abs(expansionVelocity) >= maximumExpansionFlickVelocity)
                {
                    expansionVelocity = expansionVelocity > 0 ? maximumExpansionFlickVelocity : -maximumExpansionFlickVelocity;
                }
                startFlick = IsInertiaEnabled;
                _inertiaProcessor.ExpansionBehavior.InitialVelocityX = expansionVelocity;
                _inertiaProcessor.ExpansionBehavior.InitialVelocityY = expansionVelocity;
                _inertiaProcessor.ExpansionBehavior.InitialRadius = (float)_radius;
            }
            else
            {
                _inertiaProcessor.ExpansionBehavior.InitialVelocityX = 0.0f;
                _inertiaProcessor.ExpansionBehavior.InitialVelocityY = 0.0f;
                _inertiaProcessor.ExpansionBehavior.InitialRadius = 1.0f;
            }

            if (startFlick)
            {
                _inertiaTimer.Start();
            }
        }

        /// <summary>
        /// Here when manipulation completes.
        /// </summary>
        private void OnInertiaCompleted(object sender, Manipulation2DCompletedEventArgs e)
        {
            _inertiaTimer.Stop();
        }

        /// <summary>
        /// Here when the inertia timer ticks.
        /// </summary>
        private void OnTimerTick(object sender, EventArgs e)
        {
            _inertiaProcessor.Process(Timestamp);
        }

        /// <summary>
        /// Moves the item as specified.
        /// </summary>
        private void Move(Point manipulationOrigin, Vector deltaTranslation, double deltaRotationInRadians, double deltaScale)
        {
            // apply rotation and scale constrains if needed,
            // for the scale make sure that the result radius is within Minimum..MaximumRadius interval
            deltaScale = Math.Max(Math.Min(deltaScale, MaximumScaleRadius / _radius), MinimumScaleRadius / _radius);

            // adjust translation
            if (IsTranslateEnabled)
            {
                AdjustTranslation(_center, manipulationOrigin, ref deltaTranslation, deltaRotationInRadians, deltaScale);
            }
            else
            {
                deltaTranslation = new Vector();
            }

            // apply container constrains on translation
            var parent = AssociatedObject.Parent as UIElement;
            if (parent != null)
            {
                var newCenter = deltaTranslation + _center;
                if (IsConstrainedToParentBounds)
                {
                    var parentSize = parent.RenderSize;
                    newCenter.X = Math.Max(0, Math.Min(newCenter.X, parentSize.Width));
                    newCenter.Y = Math.Max(0, Math.Min(newCenter.Y, parentSize.Height));
                }
                deltaTranslation = Vector.Subtruct(newCenter, _center);
            }

            // position the item
            double deltaRotationInDegrees = deltaRotationInRadians * 180.0 / Math.PI;
            Move(deltaTranslation + _center, deltaRotationInDegrees + _orientation, _radius * deltaScale);
        }

        /// <summary>
        /// Performs the actual move.
        /// </summary>
        /// <param name="center">
        /// The origin of manipulation
        /// </param>
        /// <param name="orientation">
        /// Represents the degree for rotation
        /// </param>
        /// <param name="scaleRadius">
        /// Represents the scaling factor
        /// </param>
        public void Move(Point center, double orientation, double scaleRadius)
        {
            // change center and manipulation pivot
            _center = center;

            // calculate transformation matrix
            var matrix = Matrix.Identity;

            if (IsRotateEnabled)
            {
                // change orientation
                _orientation = orientation;

                UpdatePivot(IsPivotEnabled);
            }

            MatrixHelper.Rotate(ref matrix, _orientation);

            // change item's width and height if scaling is enabled
            if (this.IsScaleEnabled)
            {
                _radius = scaleRadius;
                AssociatedObject.Width = 2*scaleRadius;
                AssociatedObject.Height = 2*scaleRadius;
            }

            // apply translation,
            // determine the correct offset for the render transform.
            var offset = CalculateRenderOffset(_center, _orientation, AssociatedObject.Width,
                AssociatedObject.Height, AssociatedObject.RenderTransformOrigin);
            MatrixHelper.Translate(ref matrix, offset.X, offset.Y);

            // apply RenderTransform
            AssociatedObject.RenderTransform = new MatrixTransform
            {
                Matrix = matrix
            };
        }

        /// <summary>
        ///  Determines the correct offset for a render transform on an item with the given properties.
        /// </summary>
        private static Vector CalculateRenderOffset(Point center, double orientation, double width, double height,
            Point renderTransformOrigin)
        {
            // Find the center point based on the orientation, the size of the item,
            // and the RenderTransformOrigin. This tells us exactly where the center
            // of the item is rendered with respect to the upper left corner of the
            // item's layout rect.
            var renderOrigin = new Point(width * renderTransformOrigin.X, height * renderTransformOrigin.Y);

            var matrix = Matrix.Identity;
            MatrixHelper.RotateAt(ref matrix, orientation, renderOrigin);
            var renderedCenter = matrix.Transform(new Point(width * 0.5, height * 0.5));

            // Use the rendered center to determine the desired offset for the transform.
            return Vector.Subtruct(center, renderedCenter);
        }

        /// <summary>
        /// Adjusts translation delta due to rotation and/or scale are done around manipulation origin which can be 
        /// different from the item's Center.
        /// </summary>
        private static void AdjustTranslation(Point center, Point manipulationOrigin, ref Vector deltaTranslation, double deltaRotationInRadians,
            double deltaScale)
        {
            var offsetToCenter = Vector.Subtruct(center, manipulationOrigin) + deltaTranslation;

            // Adjust item position based on change in rotation
            if (deltaRotationInRadians != 0)
            {
                var rotatedOffsetToCenter = offsetToCenter;
                rotatedOffsetToCenter.Rotate(deltaRotationInRadians);
                deltaTranslation += rotatedOffsetToCenter - offsetToCenter;
            }

            // Any scaling could cause the layout rect to shift, so adjust the translation accordingly.
            if (deltaScale != 1.0)
            {
                var scaledOffsetToCenter = offsetToCenter * deltaScale;
                deltaTranslation += scaledOffsetToCenter - offsetToCenter;
            }
        }

        /// <summary>
        /// Get the manipulations we should currently be supporting.
        /// </summary>
        private Manipulations2D SupportedManipulations
        {
            get
            {
                return (IsTranslateEnabled ? Manipulations2D.Translate : Manipulations2D.None)
                    | (IsRotateEnabled ? Manipulations2D.Rotate : Manipulations2D.None)
                    | (IsScaleEnabled ? Manipulations2D.Scale : Manipulations2D.None);
            }
        }
        #endregion

        #region Misc
        /// <summary>
        /// Updates ZIndex for all children of the item's parent and brings this item to the front.
        /// </summary>
        private void BringToFront()
        {
            var parent = AssociatedObject.Parent as Panel;
            if (parent == null)
            {
                return;
            }

            // clone and sort according to the current ZIndex, make sure that "this item" is at the end of the list
            var clone = new UIElement[parent.Children.Count];
            parent.Children.CopyTo(clone, 0);
            Array.Sort(clone, delegate(UIElement e1, UIElement e2)
            {
                if (object.ReferenceEquals(e1, AssociatedObject))
                {
                    return int.MaxValue;
                }

                if (object.ReferenceEquals(AssociatedObject, e2))
                {
                    return int.MinValue;
                }

                return Canvas.GetZIndex(e1) - Canvas.GetZIndex(e2);
            });

            // update ZIndex for all children
            for (int i = 0; i < clone.Length; i++)
            {
                Canvas.SetZIndex(clone[i], i);
            }
        }

        /// <summary>
        /// Updates pivot positions.
        /// </summary>
        /// <param name="isPivotActive"></param>
        private void UpdatePivot(bool isPivotActive)
        {
            if (_manipulationProcessor != null && _radius != 0 )
            {
                _manipulationProcessor.Pivot = isPivotActive ?
                    new ManipulationPivot2D { X = (float)_center.X, Y = (float)_center.Y, Radius = (float)_radius } : null;
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            // if Dispose is not called, the TouchHelper cleans it up on the next touch frame
            TouchHelper.EnableInput(false/*enable*/);
        }
        #endregion
    }
}

