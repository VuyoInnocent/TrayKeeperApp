using TrayKeeper.ViewModel;

namespace TrayKeeper
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Navigated += OnShellNavigated;
        }

        private void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            if (e.Current.Location.OriginalString.Contains("logout"))
            {
                var viewModel = BindingContext as AppViewModel;
                if (viewModel != null)
                {
                    viewModel.IsLoggedIn = false;
                    viewModel.FlyoutBehavior = FlyoutBehavior.Disabled;
                }
                
            }
          
        }
    }
}
