using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;
using TrayKeeper.ViewModel;

namespace TrayKeeper.Views;

public partial class EditOrderPage : ContentPage
{
	public EditOrderPage(Orders order, IOrderService orderService, Action onOrderUpdated)
	{
		InitializeComponent();
        var viewModel = new EditOrderViewModel(order,orderService,onOrderUpdated);
        BindingContext = viewModel;
    }
}