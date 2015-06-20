using Microsoft.Xaml.Interactivity;
using Multitouch.W8.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MultiTouch.Behaviors.W8
{
    /// <summary>
    /// Base class for behaviors. Do no use. Derive from Behavior&lt;T&gt; in stead "/>
    /// </summary>
    public abstract class Behavior : FrameworkElement, IBehavior
    {
        private FrameworkElement associatedObject;

        DependencyObject IBehavior.AssociatedObject
        {
            get { return associatedObject as DependencyObject; }
        }

        /// <summary>
        /// The associated object
        /// </summary>
        public FrameworkElement AssociatedObject
        {
            get
            {
                return associatedObject;
            }
            set
            {
                if (associatedObject != null)
                {
                    Detach();
                }
                DataContext = null;

                associatedObject = value;
                if (associatedObject != null)
                {
                    // FIX LocalJoost 17-08-2012 moved ConfigureDataContext to OnLoaded
                    // to prevent the app hanging on a behavior attached to an element#
                    // that's not directly loaded (like a FlipViewItem)
                    OnAttached();
                }
            }
        }

        protected virtual void OnAttached()
        {
            AssociatedObject.Unloaded += AssociatedObjectUnloaded;
            AssociatedObject.Loaded += AssociatedObjectLoaded;
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            ConfigureDataContext();
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        /// <summary>
        /// Configures data context. 
        /// Courtesy of Filip Skakun
        /// http://twitter.com/xyzzer
        /// </summary>
        private async void ConfigureDataContext()
        {
            while (associatedObject != null)
            {
                if (AssociatedObjectIsInVisualTree || IsInPopup)
                {
                    Debug.WriteLine(associatedObject.Name + " found in visual tree or popup");
                    SetBinding(
                        DataContextProperty,
                        new Binding
                        {
                            Path = new PropertyPath("DataContext"),
                            Source = associatedObject
                        });

                    return;
                }
                // Make sure not to get into a blocking loop
                await Task.Delay(100);

                Debug.WriteLine(associatedObject.Name + " Not in visual tree");
                await WaitForLayoutUpdateAsync();
            }
        }

        /// <summary>
        /// Checks if object is in visual tree
        /// Courtesy of Filip Skakun
        /// http://twitter.com/xyzzer
        /// </summary>
        private bool AssociatedObjectIsInVisualTree
        {
            get
            {
                if (associatedObject != null)
                {
                    return Window.Current.Content != null && Ancestors.Contains(Window.Current.Content);
                }
                return false;
            }
        }

        /// <summary>
        /// Checks if the object is inside a popup. It's top parent should have a parent
        /// which is a popup
        /// </summary>
        private bool IsInPopup
        {
            get
            {
                var root = Ancestors.LastOrDefault() as FrameworkElement;

                if (root != null)
                {
                    return root.Parent is Popup;
                }
                return false;
            }
        }

        /// <summary>
        /// Finds the object's associatedobject's ancestors
        /// Courtesy of Filip Skakun
        /// http://twitter.com/xyzzer
        /// </summary>
        private IEnumerable<DependencyObject> Ancestors
        {
            get
            {
                if (associatedObject != null)
                {
                    var parent = VisualTreeHelper.GetParent(associatedObject);

                    while (parent != null)
                    {
                        yield return parent;
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a task that waits for a layout update to complete
        /// Courtesy of Filip Skakun
        /// http://twitter.com/xyzzer
        /// </summary>
        /// <returns></returns>
        private async Task WaitForLayoutUpdateAsync()
        {
            await EventAsync.FromEvent<object>(
                eh => associatedObject.LayoutUpdated += eh,
                eh => associatedObject.LayoutUpdated -= eh);
        }



        void IBehavior.Attach(DependencyObject associatedObject)
        {
            //throw new System.NotImplementedException();
        }

        public virtual void Detach()
        {
            AssociatedObject.Unloaded -= AssociatedObjectUnloaded;
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
        }


        //protected virtual void OnDetaching()
        //{
        //    AssociatedObject.Unloaded -= AssociatedObjectUnloaded;
        //    AssociatedObject.Loaded -= AssociatedObjectLoaded;
        //}
    }
    //public abstract class Behavior : DependencyObject, IBehavior
    //{
       

    //    protected Behavior()
    //    {
    //    }

    //    //private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
    //    //{
    //    //    this.OnDetaching();
    //    //}



    //    protected virtual void OnDetaching()
    //    {
    //        //WindowsRuntimeMarshal.RemoveEventHandler<RoutedEventHandler>(new Action<EventRegistrationToken>(this.AssociatedObject), new RoutedEventHandler(this, (IntPtr)this.AssociatedObjectUnloaded));
    //    }

    //    private DependencyObject associatedObject;

    //    public DependencyObject AssociatedObject
    //    {
    //        get
    //        {
    //            return this.associatedObject;
    //        }
    //        set
    //        {
    //            if (this.associatedObject != null)
    //            {
    //                this.OnDetaching();
    //            }
    //            this.associatedObject = value;
    //            if (this.associatedObject != null)
    //            {
    //                this.OnAttached();
    //            }
    //        }
    //    }

    //    protected virtual void OnAttached()
    //    {
    //        DependencyObject associatedObject = this.AssociatedObject;
    //        //WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(associatedObject.add_Unloaded), new Action<EventRegistrationToken>(associatedObject.remove_Unloaded), new RoutedEventHandler(this, (IntPtr)this.AssociatedObjectUnloaded));
    //    }

    //    public void Attach(DependencyObject associatedObject)
    //    {
    //        this.AssociatedObject = associatedObject;
    //        //WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(associatedObject_uu), new Action<EventRegistrationToken>(associatedObject), new RoutedEventHandler(this, (IntPtr)this.AssociatedObjectUnloaded));
    //    }

    //    public void Detach()
    //    {
    //        //WindowsRuntimeMarshal.RemoveEventHandler<RoutedEventHandler>(new Action<EventRegistrationToken>(this.AssociatedObject), new RoutedEventHandler(this, (IntPtr)this.AssociatedObjectUnloaded));
    //    }
    //}

    //public abstract class Behavior<T> : Behavior where T : DependencyObject
    //{
    //    protected Behavior()
    //    {
    //    }

    //    public T AssociatedObject
    //    {
    //        get
    //        {
    //            return (T)base.AssociatedObject;
    //        }
    //        set
    //        {
    //            base.AssociatedObject = value;
    //        }
    //    }
    //}

    /// <summary>
    /// Base class for behaviors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behavior<T> : Behavior where T : FrameworkElement
    {
        protected Behavior()
        {
        }

        new public T AssociatedObject
        {
            get
            {
                return (T)base.AssociatedObject;
            }
            set
            {
                base.AssociatedObject = value;
            }
        }
    }

}