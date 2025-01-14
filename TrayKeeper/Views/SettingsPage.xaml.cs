using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		BindingContext = new SettingsViewModel();
	}
}