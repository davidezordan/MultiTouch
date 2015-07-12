using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Interactivity;
using MultiTouch.Behaviors.Silverlight;

namespace MultiTouch.Behaviors.WP8.Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //Initialize the items
            var multiTouchBehaviors = Interaction.GetBehaviors(image1).OfType<MultiTouchBehavior>();
            if (multiTouchBehaviors.ToList().Count > 0)
                multiTouchBehaviors.First().Move(new Point(100, 250), 45, 150);

            multiTouchBehaviors = Interaction.GetBehaviors(image2).OfType<MultiTouchBehavior>();
            if (multiTouchBehaviors.ToList().Count > 0)
                multiTouchBehaviors.First().Move(new Point(300, 300), -45, 150);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}