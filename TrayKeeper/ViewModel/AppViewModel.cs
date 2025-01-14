using System.ComponentModel;

namespace TrayKeeper.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {

        private bool _isLoggedIn;
        private bool _isLoggedOut;
        private FlyoutBehavior _flyoutBehavior;
        public AppViewModel()
        {
            IsLoggedIn = false;
            FlyoutBehavior = FlyoutBehavior.Disabled;
        }

        public FlyoutBehavior FlyoutBehavior
        {
            get => _flyoutBehavior;
            set
            {
                if (_flyoutBehavior != value)
                {
                    _flyoutBehavior = value;
                    OnPropertyChanged(nameof(FlyoutBehavior));
                }
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));

                    //// Update FlyoutBehavior based on login state
                    //FlyoutBehavior = _isLoggedIn ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
                }
            }
        }

        public bool IsLoggedOut
        {
            get => _isLoggedOut;
            set
            {
                if (_isLoggedOut != value)
                {
                    _isLoggedOut = value;
                    OnPropertyChanged(nameof(IsLoggedOut));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }    
}
