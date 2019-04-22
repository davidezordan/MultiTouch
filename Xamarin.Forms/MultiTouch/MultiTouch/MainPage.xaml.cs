using MultiTouch.Behaviors;
using System.ComponentModel;
using Xamarin.Forms;

namespace MultiTouch
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            (MyImage.Behaviors[0] as MultiTouchBehavior).OnAppearing();
        }
    }
}
