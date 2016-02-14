using System;
using MultiTouch.Extensions;
using Xamarin.Forms;

namespace MultiTouch
{
    //Uses code from original Xamarin Forms samples:
    //https://github.com/xamarin/xamarin-forms-samples/tree/master/WorkingWithGestures/PinchGesture

    public class MultiTouchBehavior : Behavior<View>
    {
        private double _currentScale = 1, _startScale = 1, _xOffset, _yOffset;

        private PinchGestureRecognizer _pinchGestureRecognizer;

        private ContentView _parent;

        private View _associatedObject;

        private void _cleanupEvents()
        {
            _pinchGestureRecognizer.PinchUpdated -= OnPinchUpdated;
            _associatedObject.BindingContextChanged -= AssociatedObjectBindingContextChanged;
        }

        private void _initializeEvents()
        {
            _cleanupEvents();
            _pinchGestureRecognizer.PinchUpdated += OnPinchUpdated;
            _associatedObject.BindingContextChanged += AssociatedObjectBindingContextChanged;
        }

        protected override void OnAttachedTo(View associatedObject)
        {
            if (associatedObject == null) return;

            _pinchGestureRecognizer = new PinchGestureRecognizer();
            _associatedObject = associatedObject;
            _initializeEvents();

            base.OnAttachedTo(associatedObject);
        }

        private void AssociatedObjectBindingContextChanged(object sender, EventArgs e)
        {
            _parent = _associatedObject.Parent as ContentView;
            _parent?.GestureRecognizers.Remove(_pinchGestureRecognizer);
            _parent?.GestureRecognizers.Add(_pinchGestureRecognizer);
        }

        protected override void OnDetachingFrom(View associatedObject)
        {
            if (associatedObject == null) return;

            _cleanupEvents();
            _parent = null;
            _pinchGestureRecognizer = null;
            _associatedObject = null;

            base.OnDetachingFrom(associatedObject);
        }

        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (_parent == null)
            {
                return;
            }

            if (e.Status == GestureStatus.Started)
            {
                _startScale = _parent.Content.Scale;
                _parent.Content.AnchorX = 0;
                _parent.Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                _currentScale += (e.Scale - 1) * _startScale;
                _currentScale = Math.Max(1, _currentScale);

                var renderedX = _parent.Content.X + _xOffset;
                var deltaX = renderedX / _parent.Width;
                var deltaWidth = _parent.Width / (_parent.Content.Width * _startScale);
                var originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                var renderedY = _parent.Content.Y + _yOffset;
                var deltaY = renderedY / _parent.Height;
                var deltaHeight = _parent.Height / (_parent.Content.Height * _startScale);
                var originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                var targetX = _xOffset - (originX * _parent.Content.Width) * (_currentScale - _startScale);
                var targetY = _yOffset - (originY * _parent.Content.Height) * (_currentScale - _startScale);

                _parent.Content.TranslationX = targetX.Clamp(-_parent.Content.Width * (_currentScale - 1), 0);
                _parent.Content.TranslationY = targetY.Clamp(-_parent.Content.Height * (_currentScale - 1), 0);

                _parent.Content.Scale = _currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                _xOffset = _parent.Content.TranslationX;
                _yOffset = _parent.Content.TranslationY;
            }
        }
    }
}
