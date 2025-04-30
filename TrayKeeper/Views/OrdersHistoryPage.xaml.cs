using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class OrdersHistoryPage : ContentPage
{
    public OrdersHistoryPage(OrdersHistoryViewModel ordersHistoryViewModel)
    {
        InitializeComponent();
        BindingContext = ordersHistoryViewModel;
    }
}