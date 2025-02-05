using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class SalesPage : ContentPage
{
    SalesViewModel _salesViewModel;

    public SalesPage(SalesViewModel salesViewModel)
	{
		InitializeComponent();
		BindingContext = _salesViewModel = salesViewModel;
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