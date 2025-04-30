using TrayKeeper.Models;
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

    private void OnInventoryTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            var selectedOrder = e.Item as Inventory;

            ((InventoryViewModel)BindingContext).EditInventoryCommand.Execute(selectedOrder);

            ((ListView)sender).SelectedItem = null;
        }
    }
}