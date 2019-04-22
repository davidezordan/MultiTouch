using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace MultiTouch.Behaviors.Silverlight.Sample
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            DataContext = new MainPageViewModel();

            Loaded += (s, a) =>
                {
                    //Initialize the items
                    var multiTouchBehaviors =
                        Interaction.GetBehaviors(item2).OfType
                            <MultiTouchBehavior>();
                    if (multiTouchBehaviors.ToList().Count > 0)
                        multiTouchBehaviors.First().Move(new Point(400, 400), 0, 150);

                    multiTouchBehaviors =
                        Interaction.GetBehaviors(item3).OfType
                            <MultiTouchBehavior>();
                    if (multiTouchBehaviors.ToList().Count > 0)
                        multiTouchBehaviors.First().Move(new Point(800, 300), -45, 200);
                };
        }

        private void btnAttach_Click(object sender, RoutedEventArgs e)
        {
            var behaviors = Interaction.GetBehaviors(item1);
            behaviors.Clear();
            var mtb = new MultiTouchBehavior
            {
                IsRotateEnabled = true,
                IsScaleEnabled = true,
                IsTranslateEnabled = true,
                IsInertiaEnabled = true,
                AreFingersVisible = true,
                MinimumScale = 20,
                MaximumScale = 500,
                IgnoreTypes = new[] { 
                    typeof(Button),
                    typeof(ToggleButton),
                    typeof(Slider)
                }
            };
            behaviors.Add(mtb);
            mtb.Move(new Point(300, 200), 45, 100);
        }

        private void btnDetach_Click(object sender, RoutedEventArgs e)
        {
            var behaviors = Interaction.GetBehaviors(item1);
            if (behaviors.Count > 0)
            {
                behaviors.Clear();
            }
        }

        private void DoubleTapBehavior_DoubleTap(object sender, EventArgs e)
        {
            //Handle your DoubleTap Event here
            //MessageBox.Show("DoubleTap Recognized.");
        }
    }
}
