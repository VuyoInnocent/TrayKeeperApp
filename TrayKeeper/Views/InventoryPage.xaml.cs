using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class InventoryPage : ContentPage
{
    public InventoryPage(InventoryViewModel inventoryViewModel)
	{
		InitializeComponent();
        BindingContext = inventoryViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var inventoryViewModel = BindingContext as InventoryViewModel;

        if (inventoryViewModel != null){
            inventoryViewModel.LoadInventory();
        }
    }
}