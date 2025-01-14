using CommunityToolkit.Maui.Alerts;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;

namespace TrayKeeper.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ILogInService _logInService;
        private string _username;
        private string _password;
        private string _message;

        public LoginViewModel(ILogInService logInService)
        {
            _logInService = logInService;
            LoginCommand = new Command(async () => await LoginAsync());
        }
        public string Username
        {
            get => _username = "Xolani";
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get => _password = "Admin";
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public ICommand LoginCommand { get; }
        private async Task LoginAsync()
        {
            var user = await _logInService.Login(Username, Password);

            if (user)
            {
               
                var viewModel = (AppViewModel)Application.Current.MainPage.BindingContext;
                viewModel.IsLoggedIn = true;
                viewModel.FlyoutBehavior = FlyoutBehavior.Flyout;
                // Navigate to the Order page
                await Shell.Current.GoToAsync("//OrderPage");
            }
            else
            {
                var toast = Toast.Make("Invalid username or password", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                await toast.Show();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
