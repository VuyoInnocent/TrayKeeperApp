using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class SalesPage : ContentPage
{
	public SalesPage(SalesViewModel salesViewModel)
	{
		InitializeComponent();
		BindingContext = salesViewModel;
	}

}