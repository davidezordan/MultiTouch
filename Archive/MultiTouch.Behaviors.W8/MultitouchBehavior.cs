using Multitouch.W8.Core;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MultiTouch.Behaviors.W8
{
    public class MultitouchBehavior : Behavior<FrameworkElement>
    {
        private Canvas elementToAnimate;
        private Dictionary<Windows.UI.Xaml.UIElement, ManipulationManager> _manipulationManager;

        public MultitouchBehavior()
            : base()
        {
            this._manipulationManager = new Dictionary<Windows.UI.Xaml.UIElement, ManipulationManager>();
        }

        protected override void OnAttached()
        {
            elementToAnimate = AssociatedObject as Canvas;
            FrameworkElement e = elementToAnimate.GetVisualChild(0);

            // Create and configure manipulation manager for this image
            // leftImage can only be rotated, while rightImage can also be translated
            var manManager = new ManipulationManager(e, elementToAnimate);

            manManager.OnFilterManipulation = ManipulationFilter.Clamp; //ManipulationFilter.ClampCenterOfMass;
            manManager.Configure(true, true, true, true);

            this._manipulationManager[e] = manManager;

            elementToAnimate.SizeChanged += OnSizeChanged;
            base.OnAttached();
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            var canvas = sender as Windows.UI.Xaml.Controls.Canvas;

            // If resized object is a canvas, update clipping geometry to its new size
            if (canvas != null)
            {
                canvas.Clip = new Windows.UI.Xaml.Media.RectangleGeometry
                {
                    Rect = new Windows.Foundation.Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight)
                };
            }
        }

        public override void Detach()
        {
            elementToAnimate.SizeChanged -= OnSizeChanged;

            base.Detach();
        }

        //#region Inertia
        //public const string InertiaPropertyName = "Inertia";

        //public int Inertia
        //{
        //    get { return (int)GetValue(InertiaProperty); }
        //    set { SetValue(InertiaProperty, value); }
        //}

        //public static readonly DependencyProperty InertiaProperty = DependencyProperty.Register(
        //    InertiaPropertyName,
        //    typeof(int),
        //    typeof(MultitouchBehavior),
        //    new PropertyMetadata(10));
        //#endregion
    }
}