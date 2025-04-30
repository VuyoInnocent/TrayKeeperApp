using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class EditOrderViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private Action _onOrderUpdated;
        private Orders _order;

        public EditOrderViewModel(Orders order, IOrderService orderService, Action onOrderUpdated)
        {
            _order = order;
            _orderService = orderService;
            _onOrderUpdated = onOrderUpdated;

            SaveCommand = new Command(SaveOrder);
        }
        private async void SaveOrder()
        {
            var messsage = string.Empty;
            // Here you can call the service to update the order
            var result = await _orderService.UpdateOrder(_order);

            if (result > 0)
            {
                messsage = "Order updated successfully!";
                _onOrderUpdated?.Invoke(); // Trigger the event
            }
            else
            {
                messsage = "Order updated not updated!";
            }
            var toast = Toast.Make(messsage, ToastDuration.Long);
            await toast.Show();
            // Close the edit page
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public string ClientName
        {
            get => _order.ClientName;
            set
            {
                _order.ClientName = value;
                OnPropertyChanged(nameof(ClientName));
            }
        }
        public string Cellphone
        {
            get => _order.Cellphone;
            set
            {
                _order.Cellphone = value;
                OnPropertyChanged(nameof(Cellphone));
            }
        }
        public string Location
        {
            get => _order.Location;
            set
            {
                _order.Location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        public int NumberOfTraysBought
        {
            get => _order.NumberTraysBought;
            set
            {
                _order.NumberTraysBought = value;
                OnPropertyChanged(nameof(NumberOfTraysBought));
            }
        }
        public bool IsPaid
        {
            get => _order.IsPaid;
            set
            {
                if (_order.IsPaid != value)
                {
                    _order.IsPaid = value;
                    OnPropertyChanged(nameof(IsPaid));
                }
            }
        }
        public bool IsCollected
        {
            get => _order.IsCollected;
            set
            {
                if (_order.IsCollected != value)
                {
                    _order.IsCollected = value;
                    OnPropertyChanged(nameof(IsCollected));
                }
            }
        }
        public ICommand SaveCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
