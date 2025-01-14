using Microsoft.Data.Sqlite;
using TrayKeeper.BL;
using TrayKeeper.ViewModel;
using TrayKeeper.Views;

namespace TrayKeeper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
