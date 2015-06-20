// ****************************************************************************
// <copyright file="MultiTouchBehavior.cs" company="Laurent Bugnion">
// Copyright © Laurent Bugnion 2010
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>10.07.2010</date>
// <project>MultiTouch.Behaviors.Silverlight</project>
// <web>http://multitouch.codeplex.com/</web>
// <license>
// See http://multitouch.codeplex.com/license.
// </license>
// <LastBaseLevel>BL0001</LastBaseLevel>
// ****************************************************************************

// ****************************************************************************
// <copyright file="MultiTouchBehavior.cs" company="Davide Zordan">
// Copyright © Davide Zordan 2010-2014
// </copyright>
// ****************************************************************************
// <author>Davide Zordan</author>
// <email>info@davidezordan.net</email>
// <date>12.08.2011</date>
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

using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using MultiTouch.Behaviors.Silverlight.Interfaces;

namespace MultiTouch.Behaviors.Silverlight
{
    /// <summary>
    /// Implements Multi-Touch Manipulation
    /// </summary>
    public partial class MultiTouchBehavior : Behavior<FrameworkElement>, IMultiTouchBehavior
    {
        // public event EventHandler<EventArgs> ManipulationStarted;

        // public event EventHandler<EventArgs> ManipulationCompleted;

#if !PHONE
        /// <summary>
        /// The <see cref="IsInertiaEnabled" /> dependency property's name.
        /// </summary>
        public const string IsInertiaEnabledPropertyName = "IsInertiaEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsInertiaEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsInertiaEnabled
        {
            get
            {
                return (bool)GetValue(IsInertiaEnabledProperty);
            }
            set
            {
                SetValue(IsInertiaEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsInertiaEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty = DependencyProperty.Register(
            IsInertiaEnabledPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnIsInertiaEnabledChanged));
#endif

        /// <summary>
        /// The <see cref="IsScaleEnabled" /> dependency property's name.
        /// </summary>
        public const string IsScaleEnabledPropertyName = "IsScaleEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsScaleEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsScaleEnabled
        {
            get
            {
                return (bool)GetValue(IsScaleEnabledProperty);
            }
            set
            {
                SetValue(IsScaleEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsScaleEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsScaleEnabledProperty = DependencyProperty.Register(
            IsScaleEnabledPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnIsScaleEnabledChanged));

        /// <summary>
        /// The <see cref="IsRotateEnabled" /> dependency property's name.
        /// </summary>
        public const string IsRotateEnabledPropertyName = "IsRotateEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsRotateEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsRotateEnabled
        {
            get
            {
                return (bool)GetValue(IsRotateEnabledProperty);
            }
            set
            {
                SetValue(IsRotateEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsRotateEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRotateEnabledProperty = DependencyProperty.Register(
            IsRotateEnabledPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnIsRotateEnabledChanged));

        /// <summary>
        /// The <see cref="IsTranslateEnabled" /> dependency property's name.
        /// </summary>
        public const string IsTranslateEnabledPropertyName = "IsTranslateEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsTranslateEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsTranslateEnabled
        {
            get
            {
                return (bool)GetValue(IsTranslateEnabledProperty);
            }
            set
            {
                SetValue(IsTranslateEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsTranslateEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTranslateEnabledProperty = DependencyProperty.Register(
            IsTranslateEnabledPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnIsTranslateEnabledChanged));

        /// <summary>
        /// The <see cref="MinimumScale" /> dependency property's name.
        /// </summary>
        public const string MinimumScalePropertyName = "MinimumScale";

        /// <summary>
        /// Gets or sets the value of the <see cref="MinimumScale" />
        /// property. This is a dependency property.
        /// </summary>
        public double MinimumScale
        {
            get
            {
                return (double)GetValue(MinimumScaleProperty);
            }
            set
            {
                SetValue(MinimumScaleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MinimumScale" /> dependency property.
        /// TODO Should coerce with MaximumScale
        /// </summary>
        public static readonly DependencyProperty MinimumScaleProperty = DependencyProperty.Register(
            MinimumScalePropertyName,
            typeof(double),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(0.5, OnMinimumScaleChanged));

        /// <summary>
        /// The <see cref="MaximumScale" /> dependency property's name.
        /// </summary>
        public const string MaximumScalePropertyName = "MaximumScale";

        /// <summary>
        /// Gets or sets the value of the <see cref="MaximumScale" />
        /// property. This is a dependency property.
        /// </summary>
        public double MaximumScale
        {
            get
            {
                return (double)GetValue(MaximumScaleProperty);
            }
            set
            {
                SetValue(MaximumScaleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumScale" /> dependency property.
        /// TODO Should coerce with MinimumScale
        /// </summary>
        public static readonly DependencyProperty MaximumScaleProperty = DependencyProperty.Register(
            MaximumScalePropertyName,
            typeof(double),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(100.0, OnMaximumScaleChanged));

        /// <summary>
        /// Identifies the <see cref="Scale" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            ScalePropertyName,
            typeof(double),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(100.0, OnScaleChanged));

        /// <summary>
        /// The <see cref="Scale" /> dependency property's name.
        /// </summary>
        public const string ScalePropertyName = "Scale";

        /// <summary>
        /// Gets or sets the value of the <see cref="Scale" />
        /// property. This is a dependency property.
        /// </summary>
        public double Scale
        {
            get
            {
                return _multiTouchManipulationBehavior.Scale;
            }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Rotation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
            RotationPropertyName,
            typeof(double),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(0.0, OnRotationChanged));

        /// <summary>
        /// The <see cref="Rotation" /> dependency property's name.
        /// </summary>
        public const string RotationPropertyName = "Rotation";

        /// <summary>
        /// Gets or sets the value of the <see cref="Rotation" />
        /// property. This is a dependency property.
        /// </summary>
        public double Rotation
        {
            get
            {
                return _multiTouchManipulationBehavior.Rotation;
            }
            set
            {
                SetValue(RotationProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CenterX" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register(
            CenterXPropertyName,
            typeof(double),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(0.0, OnCenterXChanged));

        /// <summary>
        /// The <see cref="CenterX" /> dependency property's name.
        /// </summary>
        public const string CenterXPropertyName = "CenterX";

        /// <summary>
        /// Gets or sets the value of the <see cref="CenterX" />
        /// property. This is a dependency property.
        /// </summary>
        public double CenterX
        {
            get
            {
                return _multiTouchManipulationBehavior.CenterX;
            }
            set
            {
                SetValue(CenterXProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CenterY" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register(
            CenterYPropertyName,
            typeof(double),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(0.0, OnCenterYChanged));

        /// <summary>
        /// The <see cref="CenterY" /> dependency property's name.
        /// </summary>
        public const string CenterYPropertyName = "CenterY";

        /// <summary>
        /// Gets or sets the value of the <see cref="CenterY" />
        /// property. This is a dependency property.
        /// </summary>
        public double CenterY
        {
            get
            {
                return _multiTouchManipulationBehavior.CenterY;
            }
            set
            {
                SetValue(CenterYProperty, value);
            }
        }

        /// <summary>
        /// The <see cref="IsPivotEnabled" /> dependency property's name.
        /// </summary>
        public const string IsPivotEnabledPropertyName = "IsPivotEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsPivotEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsPivotEnabled
        {
            get
            {
                return (bool)GetValue(IsPivotEnabledProperty);
            }
            set
            {
                SetValue(IsPivotEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsPivotEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPivotEnabledProperty = DependencyProperty.Register(
            IsPivotEnabledPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnIsPivotEnabledChanged));

        /// <summary>
        /// Gets or sets the value of the <see cref="IgnoreTypes" />
        /// property. This is a dependency property.
        /// </summary>
        public Type[] IgnoreTypes
        {
            get { return (Type[])GetValue(IgnoreTypesProperty); }
            set { SetValue(IgnoreTypesProperty, value); }
        }

        public static readonly DependencyProperty IgnoreTypesProperty = DependencyProperty.Register(
            "IgnoreTypes",
            typeof(Type[]),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(null, OnIgnoredTypesChanged));

        /// <summary>
        /// The <see cref="IsConstrainedToParentBounds" /> dependency property's name.
        /// </summary>
        public const string IsConstrainedToParentBoundsPropertyName = "IsConstrainedToParentBounds";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsConstrainedToParentBounds" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsConstrainedToParentBounds
        {
            get
            {
                return (bool)GetValue(IsConstrainedToParentBoundsProperty);
            }
            set
            {
                SetValue(IsConstrainedToParentBoundsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsConstrainedToParentBounds" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsConstrainedToParentBoundsProperty = DependencyProperty.Register(
            IsConstrainedToParentBoundsPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnIsConstrainedToParentBoundsChanged));

        /// <summary>
        /// The <see cref="AreManipulationsEnabled" /> dependency property's name.
        /// </summary>
        public const string AreManipulationsEnabledPropertyName = "AreManipulationsEnabled";

        /// <summary>
        /// Gets or sets the value of the <see cref="AreManipulationsEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        public bool AreManipulationsEnabled
        {
            get
            {
                return (bool)GetValue(AreManipulationsEnabledProperty);
            }
            set
            {
                SetValue(AreManipulationsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="AreManipulationsEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreManipulationsEnabledProperty = DependencyProperty.Register(
            AreManipulationsEnabledPropertyName,
            typeof(bool),
            typeof(MultiTouchBehavior),
            new PropertyMetadata(true, OnAreManipulationsEnabledChanged));

        /// <summary>
        /// Initialize the behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            OnAttachedImpl();
        }

        /// <summary>
        /// Occurs when detaching the behavior
        /// </summary>
        protected override void OnDetaching()
        {
            OnDetachingImpl();
            AssociatedObject.RenderTransform = null;
            base.OnDetaching();
        }
    }
}

