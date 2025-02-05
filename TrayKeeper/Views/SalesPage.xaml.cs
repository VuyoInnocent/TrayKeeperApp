using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class SalesPage : ContentPage
{
    public SalesPage(SalesViewModel salesViewModel)
	{
		InitializeComponent();
		BindingContext = salesViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var salesViewModel = BindingContext as SalesViewModel;

        if (salesViewModel != null)
        {
            salesViewModel.LoadSalesDetails();
        }
    }

}