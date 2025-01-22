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

            ((OrderViewModel)BindingContext).EditOrderCommand.Execute(selectedOrder);

            ((ListView)sender).SelectedItem = null;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var orderViewModel = BindingContext as OrderViewModel;

        if (orderViewModel != null) {
            orderViewModel.LoadOrders();
        }
    }
}