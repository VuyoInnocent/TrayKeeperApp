using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;
using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class EditInventoryPage : ContentPage
{
    public EditInventoryPage(IInventoryService inventoryService, Inventory inventory, Action onInventoryUpdated)
	{
		InitializeComponent();
        var viewModel = new EditInventoryViewModel(inventoryService, inventory, onInventoryUpdated);
        BindingContext = viewModel;
    }

}