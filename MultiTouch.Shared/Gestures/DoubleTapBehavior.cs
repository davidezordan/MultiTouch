// ****************************************************************************
// <copyright file="DoubleTapBehavior.cs" company="Davide Zordan">
// Copyright © Davide Zordan 2011-2014
// </copyright>
// ****************************************************************************
// <author>Davide Zordan</author>
// <email>info@davidezordan.net</email>
// <date>13.08.2011</date>
// <project>MultiTouch.Behaviors.Silverlight</project>
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

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MultiTouch.Behaviors.Silverlight.Gestures
{
    /// <summary>
    /// Implements a DoubleTap Gesture
    /// </summary>
    public class DoubleTapBehavior : Behavior<FrameworkElement>
    {
        #region Fields and constants

#if WINDOWS_PHONE

#elif SILVERLIGHT
        /// <summary>
        /// The position of the click
        /// </summary>
        Point _clickPosition;

        /// <summary>
        /// The time of the last mouse click
        /// </summary>
        public DateTime _lastClick = DateTime.Now;

        /// <summary>
        /// First click done
        /// </summary>
        private bool _firstClickDone = false;
#endif

        #endregion

        #region Public Events

        /// <summary>
        /// Public DoubleTap Event: fired when the gesture is recognized
        /// </summary>
        public event EventHandler<EventArgs> DoubleTap;

        #endregion

        #region Behavior implementation

        /// <summary>
        /// Initialize the behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // Subscribe to the DoubleTap event
#if WINDOWS_PHONE
            this.AssociatedObject.DoubleTap -= element_DoubleTap;
            this.AssociatedObject.DoubleTap += element_DoubleTap;
#elif SILVERLIGHT
            this.AssociatedObject.MouseLeftButtonDown -= element_DoubleTap;
            this.AssociatedObject.MouseLeftButtonDown += element_DoubleTap;
#endif
            // OnAttachedImpl();
        }

        /// <summary>
        /// Occurs when detaching the behavior
        /// </summary>
        protected override void OnDetaching()
        {
            // OnDetachingImpl();

            // Unsubscribe to the DoubleTap event
#if WINDOWS_PHONE
            this.AssociatedObject.DoubleTap -= element_DoubleTap;
#elif SILVERLIGHT
            this.AssociatedObject.MouseLeftButtonDown -= element_DoubleTap;
#endif
            base.OnDetaching();
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Handles a DoubleTap event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The Event Args</param>
        void element_DoubleTap(object sender, GestureEventArgs e)
        {
            if (IsDefaultBehaviorEnabled)
            {
                var multiTouchBehaviors = Interaction.GetBehaviors(sender as UIElement).OfType<MultiTouchBehavior>();
                if (multiTouchBehaviors.ToList().Count > 0)
                {
                    var mtb = multiTouchBehaviors.First();
                    mtb.Scale = mtb.Scale < mtb.MaximumScale ? mtb.MaximumScale : mtb.MinimumScale;
                }
            }
            if (this.DoubleTap != null && IsDoubleTapEventEnabled)
            {
                this.DoubleTap(this.AssociatedObject, new EventArgs());
            }
        }       
#elif SILVERLIGHT
        /// <summary>
        /// Handles a MouseDown event to recognize a DoubleTap gesture
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The Event Args</param>
        void element_DoubleTap(object sender, MouseButtonEventArgs e)
        {
            var clickTime = DateTime.Now;
            TimeSpan span = clickTime - _lastClick;
            if (span.TotalMilliseconds > 300 || _firstClickDone == false)
            {
                _clickPosition = e.GetPosition(sender as UIElement);
                _firstClickDone = true;
                _lastClick = DateTime.Now;
            }
            else
            {
                Point position = e.GetPosition(sender as UIElement);
                if (Math.Abs(_clickPosition.X - position.X) < 4 && Math.Abs(_clickPosition.Y - position.Y) < 4)
                {
                    if (IsDefaultBehaviorEnabled)
                    {
                        var multiTouchBehaviors = Interaction.GetBehaviors(sender as UIElement).OfType<MultiTouchBehavior>();
                        if (multiTouchBehaviors.ToList().Count > 0)
                        {
                            var mtb = multiTouchBehaviors.First();
                            if (mtb.Scale < mtb.MaximumScale)
                                mtb.Scale = mtb.MaximumScale;
                            else mtb.Scale = mtb.MinimumScale;
                        }
                    }

                    if (this.DoubleTap != null && IsDoubleTapEventEnabled)
                    {
                        this.DoubleTap(this.AssociatedObject, new EventArgs());
                    }
                }
                else
                {
                    _firstClickDone = false;
                }
            }
        }
#endif

        #endregion

        #region Dependency Properties

        /// <summary>
        /// The <see cref="IsDoubleTapEventEnabled" /> dependency property's name.
        /// </summary>
        public const string IsDoubleTapEventEnabledPropertyName = "IsDoubleTapEventEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsDoubleTapEventEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsDoubleTapEventEnabled
        {
            get
            {
                return (bool)GetValue(IsDoubleTapEventEnabledProperty);
            }
            set
            {
                SetValue(IsDoubleTapEventEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsDoubleTapEventEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoubleTapEventEnabledProperty = DependencyProperty.Register(
            IsDoubleTapEventEnabledPropertyName,
            typeof(bool),
            typeof(DoubleTapBehavior),
            new PropertyMetadata(true, null));

        /// <summary>
        /// The <see cref="IsDefaultBehaviorEnabled" /> dependency property's name.
        /// </summary>
        public const string IsDefaultBehaviorEnabledPropertyName = "IsDefaultBehaviorEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsDefaultBehaviorEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsDefaultBehaviorEnabled
        {
            get
            {
                return (bool)GetValue(IsDefaultBehaviorEnabledProperty);
            }
            set
            {
                SetValue(IsDefaultBehaviorEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsDefaultBehaviorEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDefaultBehaviorEnabledProperty = DependencyProperty.Register(
            IsDefaultBehaviorEnabledPropertyName,
            typeof(bool),
            typeof(DoubleTapBehavior),
            new PropertyMetadata(true, null));

        #endregion
    }
}

