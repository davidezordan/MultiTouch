using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace MultiTouch.Behaviors.Silverlight.Sample
{
    /// <summary>
    /// Simple ViewModel for the MainPage
    /// </summary>
    public class MainPageViewModel:INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            //Initialize a collection of Pictures
            Pictures = new ObservableCollection<BitmapImage>()
                           {
                               new BitmapImage(new Uri("Images/JellyFish.jpg", UriKind.RelativeOrAbsolute)),
                               new BitmapImage(new Uri("Images/Desert.jpg", UriKind.RelativeOrAbsolute)),
                               new BitmapImage(new Uri("Images/Hydrangeas.jpg", UriKind.RelativeOrAbsolute))
                           };
            SelectedPicture = Pictures.FirstOrDefault();
        }

        /// <summary>
        /// Pictures collection
        /// </summary>
        private ObservableCollection<BitmapImage> _pictures;
        public ObservableCollection<BitmapImage> Pictures
        {
            get { return _pictures; }
            set { _pictures = value; RaisePropertyChanged("Pictures"); }
        }

        /// <summary>
        /// The Selected picture
        /// </summary>
        private BitmapImage _selectedPicture;
        public BitmapImage SelectedPicture
        {
            get { return _selectedPicture; }
            set { _selectedPicture = value; RaisePropertyChanged("SelectedPicture"); }
        }

        #region INotifyPropertyChanged implementation        
            public event PropertyChangedEventHandler PropertyChanged;

            void RaisePropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        #endregion
    }
}
