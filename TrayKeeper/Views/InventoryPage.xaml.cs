using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class InventoryPage : ContentPage
{
    public InventoryPage(InventoryViewModel inventoryViewModel)
	{
		InitializeComponent();
        BindingContext = inventoryViewModel;
    }
}