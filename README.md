# Multi-Touch Behaviors

XAML Behaviors implementing Multi-Touch Manipulation (Gestures) and Inertia.

Xamarin Forms Behavior uses code from original Xamarin.Forms samples: https://github.com/xamarin/xamarin-forms-samples/tree/master/WorkingWithGestures

Archived Projects targeting Silverlight, WPF and Windows Phone 7.x, 8.x available on CodePlex: http://multitouch.codeplex.com/.

## Xamarin.Forms: using the PanGestureRecognizer
<p style="text-align: center;"><img class="size-medium wp-image-7904 aligncenter" src="https://davide.dev/wp-content/uploads/2016/02/Using-PanGestureRecognizer.png" alt="Using the PanGestureRecognizer" width="256" height="300"></p>

<p style="text-align: justify;">Recently I've blogged about Xamarin.Forms and&nbsp;how to create a XAML Behavior for enabling&nbsp;Multi-Touch gestures to generic elements and implementing a scale / pinch functionality.</p>

<p style="text-align: justify;">Fortunately the framework provides <a href="https://developer.xamarin.com/guides/xamarin-forms/user-interface/gestures/" target="_blank" rel="noopener noreferrer">three types of recognizer</a> that greatly simplify the implementation:</p>

<ul>
    <li style="text-align: justify;"><strong>PinchGestureRecognizer</strong> allows user interactions for zoom / scale functionalities;</li>
    <li style="text-align: justify;"><strong>PanGestureRecognizer</strong> enables pan / translate transformations;</li>
    <li style="text-align: justify;"><strong>TapGestureRecognizer</strong> detects tap events.</li>
</ul>

<p style="text-align: justify;">Yesterday I decided to try the&nbsp;<strong>PanGestureRecognizer&nbsp;</strong>for&nbsp;extending the capabilities of the Behavior described in the previous post.</p>

<p style="text-align: justify;">First of all, I added two <em>Bindable</em> properties in order to permit activation / deactivation of individual gestures (<em>Bindable</em> properties&nbsp;are equivalent to <em>Dependency</em> ones&nbsp;in UWP XAML)</p>

<pre class="lang:default decode:true" title="IsTranslateEnabled and IsScaleEnabled Bindable properties">#region IsScaleEnabled property

/// &lt;summary&gt;
/// Identifies the &lt;see cref="IsScaleEnabledProperty" /&gt; property.
/// &lt;/summary&gt;
public static readonly BindableProperty IsScaleEnabledProperty =
    BindableProperty.Create&lt;MultiTouchBehavior, bool&gt;(w =&gt; w.IsScaleEnabled, default(bool));

/// &lt;summary&gt;
/// Identifies the &lt;see cref="IsScaleEnabled" /&gt; dependency / bindable property.
/// &lt;/summary&gt;
public bool IsScaleEnabled
{
    get { return (bool)GetValue(IsScaleEnabledProperty); }
    set { SetValue(IsScaleEnabledProperty, value); }
}

#endregion

#region IsTranslateEnabled property

/// &lt;summary&gt;
/// Identifies the &lt;see cref="IsTranslateEnabledProperty" /&gt; property.
/// &lt;/summary&gt;
public static readonly BindableProperty IsTranslateEnabledProperty =
    BindableProperty.Create&lt;MultiTouchBehavior, bool&gt;(w =&gt; w.IsTranslateEnabled, default(bool));

/// &lt;summary&gt;
/// Identifies the &lt;see cref="IsTranslateEnabled" /&gt; dependency / bindable property.
/// &lt;/summary&gt;
public bool IsTranslateEnabled
{
    get { return (bool)GetValue(IsTranslateEnabledProperty); }
    set { SetValue(IsTranslateEnabledProperty, value); }
}

#endregion</pre>

<p style="text-align: justify;">In this way we can specify in our XAML what gestures are enabled:</p>

<pre class="lang:default decode:true ">...
&lt;Image.Behaviors&gt;
     &lt;behaviors:MultiTouchBehavior IsScaleEnabled="True" IsTranslateEnabled="True" /&gt;
&lt;/Image.Behaviors&gt;
...</pre>

<p style="text-align: justify;">Then I initialised the GestureRecognizers adding a new&nbsp;<strong>PanGestureRecognizer&nbsp;</strong>to the recognizers list:</p>

<pre class="lang:default decode:true ">/// &lt;summary&gt;
/// Initialise the Gesture Recognizers.
/// &lt;/summary&gt;
private void InitialiseRecognizers()
{
    _panGestureRecognizer = new PanGestureRecognizer();
}

/// &lt;summary&gt;
/// Occurs when BindingContext is changed: used to initialise the Gesture Recognizers.
/// &lt;/summary&gt;
/// &lt;param name="sender"&gt;The sender object.&lt;/param&gt;
/// &lt;param name="e"&gt;The event parameters.&lt;/param&gt;
private void AssociatedObjectBindingContextChanged(object sender, EventArgs e)
{
    _parent = _associatedObject.Parent as ContentView;
    _parent?.GestureRecognizers.Remove(_panGestureRecognizer);
    _parent?.GestureRecognizers.Add(_panGestureRecognizer);
}</pre>

<p style="text-align: justify;">And subscribed to the <strong>PanUpdated</strong> event in order to apply the translate transform:</p>

<pre class="lang:default decode:true ">/// &lt;summary&gt;
/// Initialise the events.
/// &lt;/summary&gt;
private void InitializeEvents()
{
    CleanupEvents();

    _panGestureRecognizer.PanUpdated += OnPanUpdated;
}</pre>

<p style="text-align: justify;">The implementation of this event handler permits to update the X and Y coordinates&nbsp;of the element when a Pan gesture is detected:</p>

<pre class="lang:default decode:true " title="PanUpdated event handler implementation">/// &lt;summary&gt;
/// Implements Pan/Translate.
/// &lt;/summary&gt;
/// &lt;param name="sender"&gt;The sender object.&lt;/param&gt;
/// &lt;param name="e"&gt;The event parameters.&lt;/param&gt;
private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
{
    if (_parent == null)
    {
        return;
    }

    if (!IsTranslateEnabled)
    {
        return;
    }

    switch (e.StatusType)
    {
        case GestureStatus.Running:
            _parent.Content.TranslationX = _xOffset + e.TotalX;
            _parent.Content.TranslationY = _yOffset + e.TotalY;
            break;

        case GestureStatus.Completed:
            _xOffset = _parent.Content.TranslationX;
            _yOffset = _parent.Content.TranslationY;
            break;
    }
}</pre>

<p style="text-align: justify;">Here we go: the sample app can now be deployed to the emulators and iOS / Android / Windows devices.</p>

<p style="text-align: justify;">Just a couple of notes:</p>

<ul>
    <li style="text-align: justify;">deployment to iOS required this <a href="http://codeworks.it/blog/?p=242" target="_blank" rel="noopener noreferrer">workaround</a>&nbsp;to work properly since the new sample app uses different assemblies;</li>
    <li style="text-align: justify;">Project has been updated to Visual Studio 2019 and Xamarin.Forms 3.6.0.</li>
</ul>

