using TrayKeeper.Models;
using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class OrderPage : ContentPage
{
    OrderViewModel _orderViewModel;

    public OrderPage(OrderViewModel orderViewModel)
	{
		InitializeComponent();
		BindingContext = _orderViewModel = orderViewModel;
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
    private void OnClientNameTextChanged(object sender, TextChangedEventArgs e)
    {
     
         _orderViewModel.ClientName = e.NewTextValue;
        
    }
    private void OnClientNameSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            var selectedClientName = e.CurrentSelection[0] as string;
            _orderViewModel.ClientName = selectedClientName;
            _orderViewModel.IsListVisible = false; 
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