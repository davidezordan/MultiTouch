// ****************************************************************************
// <copyright file="MultiTouchBehavior.Silverlight.cs" company="Davide Zordan">
// Copyright © Davide Zordan 2010-2014
// </copyright>
// ****************************************************************************
// <author>Davide Zordan</author>
// <email>info@davidezordan.net</email>
// <date>12.08.2011</date>
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
using System.Windows;
using System.Windows.Input.Manipulations;

namespace MultiTouch.Behaviors.Silverlight
{
        /// <summary>
        /// Implements Multi-Touch Manipulation
        /// </summary>
        public partial class MultiTouchBehavior
        {
            private MultiTouchManipulationBehavior _multiTouchManipulationBehavior;

            private void OnAttachedImpl()
            {
                _multiTouchManipulationBehavior=new MultiTouchManipulationBehavior();
                _multiTouchManipulationBehavior.Attach(AssociatedObject);
                _multiTouchManipulationBehavior.IsPivotEnabled = IsPivotEnabled;
                _multiTouchManipulationBehavior.IsInertiaEnabled = IsInertiaEnabled;
                _multiTouchManipulationBehavior.AreFingersVisible = AreFingersVisible;
                _multiTouchManipulationBehavior.IsRotateEnabled = IsRotateEnabled;
                _multiTouchManipulationBehavior.IsScaleEnabled = IsScaleEnabled;
                _multiTouchManipulationBehavior.IsTranslateEnabled = IsTranslateEnabled;
                _multiTouchManipulationBehavior.MaximumScaleRadius = MaximumScale;
                _multiTouchManipulationBehavior.MinimumScaleRadius = MinimumScale;
                _multiTouchManipulationBehavior.IsConstrainedToParentBounds = IsConstrainedToParentBounds;
                _multiTouchManipulationBehavior.IgnoredTypes = IgnoreTypes;
                _multiTouchManipulationBehavior.AreManipulationsEnabled = AreManipulationsEnabled;
            }

            private void OnDetachingImpl()
            {
                if (_multiTouchManipulationBehavior!=null)
                {
                    _multiTouchManipulationBehavior.Detach();
                }
            }

            private static void OnIsInertiaEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior) sender)._multiTouchManipulationBehavior!=null))
                {
                    ((MultiTouchBehavior) sender)._multiTouchManipulationBehavior.IsInertiaEnabled = (bool) e.NewValue;
                }
            }

            private static void OnIsScaleEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.IsScaleEnabled = (bool)e.NewValue;
                }
            }

            private static void OnIsRotateEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.IsRotateEnabled = (bool)e.NewValue;
                }
            }

            private static void OnIsTranslateEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.IsTranslateEnabled = (bool)e.NewValue;
                }
            }

            private static void OnMinimumScaleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.MinimumScaleRadius = (int)e.NewValue*3.6;
                }
            }

            private static void OnMaximumScaleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.MaximumScaleRadius = (int)e.NewValue * 3.6;
                }
            }

            private static void OnIsPivotEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.IsPivotEnabled = (bool)e.NewValue;
                }
            }

            private static void OnIgnoredTypesChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.IgnoredTypes = (Type[])e.NewValue;
                }
            }

            private static void OnIsConstrainedToParentBoundsChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.IsConstrainedToParentBounds = (bool)e.NewValue;
                }
            }

            private static void OnAreManipulationsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if ((sender is MultiTouchBehavior) && (e.NewValue != null) && (((MultiTouchBehavior)sender)._multiTouchManipulationBehavior != null))
                {
                    ((MultiTouchBehavior)sender)._multiTouchManipulationBehavior.AreManipulationsEnabled = (bool)e.NewValue;
                }
            }

            private static void OnScaleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                var mtb = (sender as MultiTouchBehavior);
                if (mtb == null || e.NewValue == null || mtb._multiTouchManipulationBehavior == null) return;
                var newValue = (double)e.NewValue;
                var minScaleValue = mtb.MinimumScale;
                var maxScaleValue = mtb.MaximumScale;
                var scaleValue = newValue;
                if (newValue < minScaleValue)
                    scaleValue = minScaleValue;
                else if (scaleValue > maxScaleValue)
                    scaleValue = maxScaleValue;

                mtb.Move(new Point(mtb.CenterX, mtb.CenterY), mtb.Rotation, (double)scaleValue);
            }

            private static void OnRotationChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                var mtb = (sender as MultiTouchBehavior);
                if (mtb == null || e.NewValue == null || mtb._multiTouchManipulationBehavior == null || !mtb.IsRotateEnabled) return;
                mtb.Move(new Point(mtb.CenterX, mtb.CenterY), (double)e.NewValue, mtb.Scale);
            }

            private static void OnCenterXChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                var mtb = (sender as MultiTouchBehavior);
                if (mtb == null || e.NewValue == null || mtb._multiTouchManipulationBehavior == null || !mtb.IsRotateEnabled) return;
                mtb.Move(new Point((double)e.NewValue, mtb.CenterY), mtb.Rotation, mtb.Scale);
            }

            private static void OnCenterYChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                var mtb = (sender as MultiTouchBehavior);
                if (mtb == null || e.NewValue == null || mtb._multiTouchManipulationBehavior == null || !mtb.IsRotateEnabled) return;
                mtb.Move(new Point(mtb.CenterX, (double)e.NewValue), mtb.Rotation, mtb.Scale);
            }

            private bool _isDebugModeActive;
            public bool IsDebugModeActive
            {
                get
                {
                    return _isDebugModeActive;
                }
                set
                {
#if DEBUG
                    _isDebugModeActive = value;
#endif
                }
            }

            private bool _areFingersVisible;
            public bool AreFingersVisible
            {
                get
                {
                    return _areFingersVisible;
                }
                set
                {
#if DEBUG
                    _areFingersVisible = value;
#endif
                }
            }

            /// <summary>
            /// Performs the actual move.
            /// </summary>
            public void Move(Point center, double rotation, double scaleRadius)
            {
                if (_multiTouchManipulationBehavior != null)
                {
                    _multiTouchManipulationBehavior.Move(center, rotation, scaleRadius);
                }
            }

            /// <summary>
            /// The Manipulation processor
            /// </summary>
            public ManipulationProcessor2D ManipulationProcessor
            {
                get
                {
                    return this._multiTouchManipulationBehavior == null ? null : this._multiTouchManipulationBehavior.ManipulationProcessor;
                }
            }
        }
}
