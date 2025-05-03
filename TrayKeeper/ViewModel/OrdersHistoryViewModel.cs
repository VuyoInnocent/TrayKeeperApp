using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class OrdersHistoryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Orders> Orders { get; set; }
        private ObservableCollection<Orders> _filteredOrders;
        private IOrderService _orderService;
        private string? _filterBatchNumber;
        private string _filterClientName;
        private DateTime _filterOrderDate;
        private bool _isNoOrdersFound;

        public OrdersHistoryViewModel(IOrderService orderService)
        {
            Orders = new ObservableCollection<Orders>();
            ApplyFiltersCommand = new Command(ApplyFilters);
            ClearFiltersCommand = new Command(ClearFilters);
            RefreshCommand = new Command(RefreshOrders);
            SetTodayCommand = new Command(() => FilterOrderDate = DateTime.Today);
            FilterOrderDate = DateTime.MinValue;
            _orderService = orderService;
            LoadOrders();
        }
        public async void LoadOrders()
        {
            try
            {
                var orders = await _orderService.GetOrders();
            
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
                FilteredOrders = new ObservableCollection<Orders>(Orders);
            }
            catch (Exception ex)
            {

            }
            

        }
        private void ApplyFilters()
        {

            var filtered = Orders.Where(o =>
                      (string.IsNullOrEmpty(FilterBatchNumber) || o.BatchNumber.ToString().Contains(FilterBatchNumber.Trim(), StringComparison.OrdinalIgnoreCase)))
                      .Where(o => (string.IsNullOrEmpty(FilterClientName) || o.ClientName.Contains(FilterClientName.Trim(), StringComparison.OrdinalIgnoreCase)))
                      .Where(o => (FilterOrderDate == DateTime.MinValue || o.DateOrdered.Date == FilterOrderDate.Date))
                      .ToList();

            FilteredOrders = new ObservableCollection<Orders>(filtered);
            IsNoOrdersFound = FilteredOrders.Count == 0;
        }
        private void ClearFilters()
        {
            FilterBatchNumber = string.Empty;
            FilterClientName = string.Empty;
            FilterOrderDate = DateTime.MinValue;

            // Reset the filtered list to show all orders
            FilteredOrders = new ObservableCollection<Orders>(Orders);
            IsNoOrdersFound = FilteredOrders.Count == 0;
        }
        private void RefreshOrders()
        {
            // Simulate refreshing the orders list (e.g., fetching from a database or API)
            // For now, we'll just reset the list to the original orders
            FilteredOrders = new ObservableCollection<Orders>(Orders);
            IsNoOrdersFound = FilteredOrders.Count == 0;
        }
        public ObservableCollection<Orders> FilteredOrders
        {
            get => _filteredOrders;
            set
            {
                _filteredOrders = value;
                OnPropertyChanged(nameof(FilteredOrders));
            }
        }
        public string? FilterBatchNumber
        {
            get => _filterBatchNumber;
            set
            {
                _filterBatchNumber = value;
                OnPropertyChanged(nameof(FilterBatchNumber));
            }
        }
        public string FilterClientName
        {
            get => _filterClientName;
            set
            {
                _filterClientName = value;
                OnPropertyChanged(nameof(FilterClientName));
            }
        }
        public DateTime FilterOrderDate
        {
            get => _filterOrderDate;
            set
            {
                _filterOrderDate = value;
                OnPropertyChanged(nameof(FilterOrderDate));
            }
        }
        public bool IsNoOrdersFound
        {
            get => _isNoOrdersFound;
            set
            {
                _isNoOrdersFound = value;
                OnPropertyChanged(nameof(IsNoOrdersFound));
            }
        }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SetTodayCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
