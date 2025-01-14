using TrayKeeper.Models;
using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class OrderPage : ContentPage
{
	public OrderPage(OrderViewModel orderViewModel)
	{
		InitializeComponent();
		BindingContext =  orderViewModel;
	}
    private void OnOrderTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            var selectedOrder = e.Item as Orders;

            // Execute the EditOrderCommand with the selected order
            ((OrderViewModel)BindingContext).EditOrderCommand.Execute(selectedOrder);

            // Optionally deselect the item
            ((ListView)sender).SelectedItem = null;
        }
    }
}