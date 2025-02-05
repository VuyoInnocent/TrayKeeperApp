using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;
using TrayKeeper.Views;

namespace TrayKeeper.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Orders> Orders { get; set; }
        private ObservableCollection<string> _filteredClientNames;
        private readonly IInventoryService _inventoryService;
        private ObservableCollection<string> _inventoryNumbers;
        private readonly IOrderService _orderService;
        private string? _selectedBatchNumber;
        private int? _numberTraysBought;
        private bool _isListVisible;
        private string? _clientName;
        private string? _cellphone;
        private string? _location;
   
        public ICommand AddOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public OrderViewModel(IOrderService orderService, IInventoryService inventoryService)
        {
            Orders = new ObservableCollection<Orders>();
            _inventoryNumbers = new ObservableCollection<string>();
            FilteredClientNames = new ObservableCollection<string>(Orders.Select(x => x.ClientName));
            AddOrderCommand = new Command(AddOrder);
            EditOrderCommand = new Command<Orders>(OnEditOrder);
            _inventoryService = inventoryService;
            _orderService = orderService;
            IsListVisible = false;
            LoadOrders();
        }
        public async void AddOrder()
        {
            var message = string.Empty;
            // Validate inputs
            if (string.IsNullOrWhiteSpace(ClientName) ||
                string.IsNullOrWhiteSpace(Cellphone) ||
                string.IsNullOrWhiteSpace(Location) ||
                string.IsNullOrWhiteSpace(_selectedBatchNumber) ||
                NumberOfTraysBought <= 0)
            {
                // Optionally show a message to the user that input is invalid
                var toast2 = Toast.Make("Enter all values", ToastDuration.Long, 30);
                await toast2.Show();
                return;
            }

            if (Cellphone.Length != 10)
            {
                var toast2 = Toast.Make("Invalid Cellphone number", ToastDuration.Long, 30);
                await toast2.Show();
                return;
            }

            // Create a new order object
            var newOrder = new Orders
            {
                ClientName = ClientName,
                Cellphone = Cellphone,
                Location = Location,
                NumberTraysBought = NumberOfTraysBought,
                DateOrdered = DateTime.Now,
            };

            var getInventory = await _inventoryService.GetInventory();
            int inventoryId = int.Parse(_selectedBatchNumber);
            var existingBatch = getInventory.Where(x => x.InventoryNumber == inventoryId).FirstOrDefault();


            if (existingBatch?.NumberOfTraysBought >= newOrder.NumberTraysBought)
            {
                existingBatch.NumberOfTraysBought = existingBatch.NumberOfTraysBought - newOrder.NumberTraysBought;
                existingBatch.NumberOfTraysSold += newOrder.NumberTraysBought;

                await _orderService.AddOrder(newOrder);
                await _inventoryService.UpdateInventory(existingBatch);
                LoadOrders();
                clear();

                message = "Order capture successfully!!";

            }
            else {
                message = $"Order not capture successfull, only ({getInventory}) trays left!";
            }
            var toast = Toast.Make(message, ToastDuration.Long, 30);
            await toast.Show();
      

        }
        private async void OnEditOrder(Orders order)
        {
            if (order == null) return;

            var editPage = new EditOrderPage(order, _orderService, LoadOrders);

            await Application.Current.MainPage.Navigation.PushAsync(editPage);
        }
        public async void LoadOrders()
        {
            var orders = await _orderService.GetOrders();
            var inventory = await _inventoryService.GetInventory();
            Orders.Clear();
            _inventoryNumbers.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }

            foreach (var item in inventory)
            {
                if (item.NumberOfTraysBought > 0 )
                {
                    _inventoryNumbers.Add(item.InventoryNumber + "");
                }
          
            }
        }
        private void FilterClientNames()
        {
            if (string.IsNullOrWhiteSpace(ClientName))
            {
                FilteredClientNames = new ObservableCollection<string>(Orders.Select(x => x.ClientName));
                IsListVisible = false; // Hide the list if input is empty
            }
            else
            {
                var filtered = Orders.Select(x => x.ClientName)
                    .Where(name => name.ToLower().Contains(ClientName.ToLower()))
                    .ToList();

                FilteredClientNames = new ObservableCollection<string>(filtered);
                IsListVisible = filtered.Any(); // Show the list if there are results
            }
        }
        public void clear()
        {
            ClientName = string.Empty;  // Clear ClientName
            Cellphone = string.Empty;    // Clear Cellphone
            Location = string.Empty;      // Clear Location
            NumberOfTraysBought = 0;     // Reset NumberOfTraysBought to 0
        }
        public string ClientName
        {
            get => _clientName;
            set
            {
                if (_clientName != value && value.Length > 2)
                {
                    _clientName = value;
                    OnPropertyChanged(nameof(ClientName));
                    FilterClientNames();
                }
             
            }
        }
        public string Cellphone
        {
            get => _cellphone;
            set
            {
                _cellphone = value;
                OnPropertyChanged(nameof(Cellphone));
            }
        }
        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        public int NumberOfTraysBought
        {
            get => _numberTraysBought.GetValueOrDefault();
            set
            {
                _numberTraysBought = value;
                OnPropertyChanged(nameof(NumberOfTraysBought));
            }
        }
        public bool IsListVisible
        {
            get => _isListVisible;
            set
            {
                _isListVisible = value;
                OnPropertyChanged(nameof(IsListVisible));
            }
        }
        public ObservableCollection<string> InventoryNumber
        {
            get => _inventoryNumbers;
            set
            {
                _inventoryNumbers = value;
                OnPropertyChanged(nameof(InventoryNumber));
            }
        }
        public ObservableCollection<string> FilteredClientNames
        {
            get => _filteredClientNames;
            set
            {
                _filteredClientNames = value;
                OnPropertyChanged(nameof(FilteredClientNames));
            }
        }
        public string SelectedBatchNumber
        {
            get => _selectedBatchNumber;
            set
            {
                _selectedBatchNumber = value;
                OnPropertyChanged(nameof(SelectedBatchNumber));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
