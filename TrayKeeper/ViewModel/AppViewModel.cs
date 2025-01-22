using System.ComponentModel;
using System.Windows.Input;

namespace TrayKeeper.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private bool _isLoggedIn;
        private FlyoutBehavior _flyoutBehavior;
        public ICommand LogoutCommand { get; }
        public AppViewModel()
        {
            IsLoggedIn = false;
            FlyoutBehavior = FlyoutBehavior.Disabled;
            LogoutCommand = new Command(Logout);
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

                }
            }
        }

        public void Logout()
        {
            IsLoggedIn = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }    
}
