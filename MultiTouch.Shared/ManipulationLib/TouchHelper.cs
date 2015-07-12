//Microsoft Surface Manipulations and Inertia Sample for Microsoft Silverlight
//http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=4b281bde-9b01-4890-b3d4-b3b45ca2c2e4
//Overview
//Multitouch support in Windows 7 allows applications to blur the lines between computers and the real 
//world. Touch-optimized applications entice users to touch the objects on the screen, drag them across 
//the screen, rotate and resize them, and flick them across the screen by using their fingers. 
//The manipulations and inertia processor classes allow graphical user interface (GUI) components to move 
//in a natural and intuitive way. Manipulations enable users to move, rotate, and resize components by 
//using their fingers. Inertia enables users to move components by applying forces on the components, 
//such as flicking the component across the screen. The contents of this sample are covered by the 
//Microsoft Surface SDK 1.0 SP1 license agreement, with any additional restrictions noted in the Readme 
//file. The purpose of this download is educational use only and is made available "as-is" without support.

//---------------------------------------------------------------------
// <copyright file="TouchHelper.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

// ****************************************************************************
// <copyright file="TouchHelper.cs" company="Davide Zordan">
// Copyright © Davide Zordan 2010-2014
// </copyright>
// ****************************************************************************
// <author>Davide Zordan</author>
// <email>info@davidezordan.net</email>
// <date>01.01.2011</date>
// <project>MultiTouch.ManipulationLib.Silverlight4</project>
// <web>http://multitouch.codeplex.com/</web>
// <license>
// See http://multitouch.codeplex.com/license.
// </license>
// <LastBaseLevel>BL0001</LastBaseLevel>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultiTouch.ManipulationLib.Silverlight
{
    

    /// <summary>
    /// Passes data for TouchReported event.
    /// </summary>
    public class TouchReportedEventArgs: EventArgs
    {
        internal TouchReportedEventArgs(IEnumerable<TouchPoint> touchPoints)
        {
            TouchPoints = touchPoints;
        }

        /// <summary>
        /// Returns reported touch points. 
        /// </summary>
        public IEnumerable<TouchPoint> TouchPoints { get; private set; }
    }


    /// <summary>
    /// Passes data for touch events.
    /// </summary>
    public class TouchEventArgs : EventArgs
    {
        internal TouchEventArgs(TouchPoint touchPoint)
        {
            TouchPoint = touchPoint;
        }

        /// <summary>
        /// Returns the associated touch point.
        /// </summary>
        public TouchPoint TouchPoint { get; private set; }
    }


    /// <summary>
    /// A group of touch event handlers.
    /// </summary>
    public class TouchHandlers
    {
        public EventHandler<TouchEventArgs> TouchDown { get; set; }
        public EventHandler<TouchEventArgs> CapturedTouchUp { get; set; }
        public EventHandler<TouchReportedEventArgs> CapturedTouchReported { get; set; }
        public EventHandler<TouchEventArgs> LostTouchCapture { get; set; }
    }

    /// <summary>
    /// A helper class to process, deliver and capture touch related events.
    /// Note: the class is not thread safe.
    /// </summary>
    public static class TouchHelper
    {
        // store the rootNode
        private static UIElement rootNode;

        // indicates if touch input is enabled or not
        private static bool isEnabled;

        // current event handlers
        private static readonly Dictionary<UIElement, TouchHandlers> currentHandlers = 
            new Dictionary<UIElement, TouchHandlers>();

        // current captured touch devices (touchDevice.Id -> capturing UIElement)
        private static readonly Dictionary<int, UIElement> currentCaptures = new Dictionary<int, UIElement>();

        // current touch points (for captured touch devices only)
        private static readonly Dictionary<int, TouchPoint> currentTouchPoints = new Dictionary<int, TouchPoint>();
 
        // an empty array of TouchPoints
        private static readonly TouchPoint[] emptyTouchPoints = new TouchPoint[0];

        /// <summary>
        /// Returns true if there is at least one touch over the root. Otheriwse - false.
        /// </summary>
        public static bool AreAnyTouches
        {
            get
            {
                return currentTouchPoints.Count != 0;
            }
        }

        /// <summary>
        /// Captured the given touchDevice. To release capture, pass element=null.
        /// </summary>
        /// <param name="touchDevice"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool Capture(this TouchDevice touchDevice, UIElement element)
        {
            if (touchDevice == null)
            {
                throw new ArgumentNullException("element");
            }

            // raise LostCapture if the capturing element is different than the existing one
            UIElement existingCapture;
            if (currentCaptures.TryGetValue(touchDevice.Id, out existingCapture) &&
                !object.ReferenceEquals(existingCapture, element))
            {
                // check if a handler exists
                TouchHandlers handlers;
                if (currentHandlers.TryGetValue(existingCapture, out handlers))
                {
                    EventHandler<TouchEventArgs> handler = handlers.LostTouchCapture;
                    if (handler != null)
                    {
                        // raise LostCapture with the last known touchPoint
                        TouchPoint touchPoint;
                        if (currentTouchPoints.TryGetValue(touchDevice.Id, out touchPoint))
                        {
                            handler(existingCapture, new TouchEventArgs(touchPoint));
                        }
                    }
                }
            }

            // update currentCaptures dictionary
            if (element != null)
            {
                // capture
                currentCaptures[touchDevice.Id] = element;
            }
            else
            {
                // release
                currentCaptures.Remove(touchDevice.Id);
            }

            return true;
        }

        /// <summary>
        /// Adds event handlers for the given UIElement. Note: the method overrides all touch handler for the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="handlers"></param>
        public static void AddHandlers(UIElement element, TouchHandlers handlers)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            currentHandlers[element] = handlers;
        }

        /// <summary>
        /// Removes event handlers from the given element.
        /// </summary>
        /// <param name="element"></param>
        public static void RemoveHandlers(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            currentHandlers.Remove(element);
        }

        /// <summary>
        /// Enables or disables touch input.
        /// </summary>
        /// <param name="enable"></param>
        public static void EnableInput(bool enable)
        {
            if (enable)
            {
                if (!isEnabled)
                {
                    EnableInput();
                    isEnabled = true;
                }
            }
            else
            {
                if (isEnabled)
                {
                    DisableInput();
                    isEnabled = false;
                }
            }
        }

        /// <summary>
        /// Enables touch input.
        /// </summary>
        private static void EnableInput()
        {
            Touch.FrameReported += TouchFrameReported;
        }

        /// <summary>
        /// Disables touch input and clear all dictionaries.
        /// </summary>
        private static void DisableInput()
        {
            Touch.FrameReported -= TouchFrameReported;
            ResetTouchPoints();
            currentHandlers.Clear();
        }

        /// <summary>
        /// Clear all dictionaries: TouchPoints and Captures.
        /// </summary>
        public static void ResetTouchPoints()
        {
            currentCaptures.Clear();
            currentTouchPoints.Clear();
        }

        /// <summary>
        /// Handles TouchFrameReported event and raise TouchDown/Up/Move events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            // get the root

            UIElement root = rootNode;
            if (root == null)
            {
                return;
            }

            foreach (TouchPoint touchPoint in e.GetTouchPoints(null))
            {
                int id = touchPoint.TouchDevice.Id;

                // check if the touchDevice is captured or not.
                UIElement captured;
                currentCaptures.TryGetValue(id, out captured);

                switch (touchPoint.Action)
                {
                    // TouchDown
                    case TouchAction.Down:
                        HitTestAndRaiseDownEvent(root, touchPoint);
                        currentTouchPoints[id] = touchPoint;
                        break;

                    // TouchUp
                    case TouchAction.Up:
                        // handle only captured touches
                        if (captured != null)
                        {
                            RaiseUpEvent(captured, touchPoint);

                            // release capture
                            Capture(touchPoint.TouchDevice, null);
                            captured = null;
                        }
                        currentTouchPoints.Remove(id);
                        break;

                    // TouchMove
                    case TouchAction.Move:
                        // just remember the new touchPoint, the event will be raised in bulk later
                        currentTouchPoints[id] = touchPoint;
                        break;
                }
            }

            // raise CapturedReportEvents
            RaiseCapturedReportEvent();
        }

        /// <summary>
        /// Iterates through all event handlers, combines all touches captured by the corresponding UIElement
        /// and raise apturedReportEvent.
        /// </summary>
        private static void RaiseCapturedReportEvent()
        {
            // walk through all handlers
            foreach (KeyValuePair<UIElement, TouchHandlers> pairHandler in currentHandlers)
            {
                EventHandler<TouchReportedEventArgs> handler = pairHandler.Value.CapturedTouchReported;
                if (handler == null)
                {
                    continue;
                }

                // walk through all touch devices captured by the current UIElement
                List<TouchPoint> capturedTouchPoints = null;
                foreach (KeyValuePair<int, UIElement> pairCapture in currentCaptures)
                {
                    if (!object.ReferenceEquals(pairCapture.Value, pairHandler.Key))
                    {
                        continue;
                    }

                    // add the captured touchPoint
                    TouchPoint capturedTouchPoint;
                    if (currentTouchPoints.TryGetValue(pairCapture.Key, out capturedTouchPoint))
                    {
                        if (capturedTouchPoints == null)
                        {
                            capturedTouchPoints = new List<TouchPoint>();
                        }
                        capturedTouchPoints.Add(capturedTouchPoint);
                    }
                }

                // raise event
                handler(pairHandler.Key, new TouchReportedEventArgs(
                    capturedTouchPoints ?? (IEnumerable<TouchPoint>)emptyTouchPoints));
            }
        }

        /// <summary>
        /// Performs hit testing, find the first element in the parent chain that has TouchDown event handler and
        /// raises TouchTouch event.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="touchPoint"></param>
        private static void HitTestAndRaiseDownEvent(UIElement root, TouchPoint touchPoint)
        {
            foreach (UIElement element in InputHitTest(root, touchPoint.Position))
            {
                TouchHandlers handlers;
                if (currentHandlers.TryGetValue(element, out handlers))
                {
                    EventHandler<TouchEventArgs> handler = handlers.TouchDown;
                    if (handler != null)
                    {
                        // call the first found handler and break
                        handler(element, new TouchEventArgs(touchPoint));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Performs hit testing and returns a collection of UIElement the given point is within.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static IEnumerable<UIElement> InputHitTest(UIElement root, Point position)
        {
            foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(position, root))
            {
                yield return element;

                for (UIElement parent = VisualTreeHelper.GetParent(element) as UIElement;
                     parent != null;
                     parent = VisualTreeHelper.GetParent(parent) as UIElement)
                {
                    yield return parent;
                }
            }
        }

        /// <summary>
        /// Raises TouchUp event.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="touchPoint"></param>
        private static void RaiseUpEvent(UIElement element, TouchPoint touchPoint)
        {
            TouchHandlers handlers;
            if (currentHandlers.TryGetValue(element, out handlers))
            {
                EventHandler<TouchEventArgs> handler = handlers.CapturedTouchUp;
                if (handler != null)
                {
                    handler(element, new TouchEventArgs(touchPoint));
                }
            }
        }

        /// <summary>
        /// Get the root element
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static UIElement GetRootElement(UIElement node)
        {
            //Find the root element
            UIElement root = node;
            do { root = VisualTreeHelper.GetParent(root) as UIElement; } while (VisualTreeHelper.GetParent(root) != null);
            return root;
        }

        /// <summary>
        /// Get the root element
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static void SetRootElement(UIElement node)
        {
            rootNode = node;
        }
    }
}
