using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Inventory> InventoryRecords { get; set; }
        private readonly IInventoryService _inventoryService;
        public int _inventoryNumber;
        public int _numberOfTraysBought;
        public int _numberOfDamagedTrays;
        public int _numberOfTraysSold;
        public DateTime _date;
        public ICommand SaveInventoryCommand { get; }
        public InventoryViewModel(IInventoryService inventoryService)
        {
            InventoryRecords = new ObservableCollection<Inventory>();
            SaveInventoryCommand = new Command(SaveInventory);
            _inventoryService = inventoryService;
            Date = DateTime.Now;

            LoadInventory();
        }
        private async void SaveInventory()
        {
            var message = string.Empty;
            var inventory = new Inventory
            {
                NumberOfTraysBought = NumberOfTraysBought,
                NumberOfDamagedTrays = NumberOfDamagedTrays,
                NumberOfTraysSold = NumberOfTraysSold,
                Date = DateTime.Now
            };

            if (string.IsNullOrWhiteSpace(NumberOfTraysBought+"") ||
             string.IsNullOrWhiteSpace(NumberOfDamagedTrays + "") ||
             string.IsNullOrWhiteSpace(NumberOfTraysSold + "") ||
             string.IsNullOrWhiteSpace(Date + ""))
            {
       
                var toast2 = Toast.Make("Enter all values", ToastDuration.Long, 30);
                await toast2.Show();
                return;
            }

            var result = await _inventoryService.AddInventory(inventory);

            if (result > 0)
            {
                message = "Inventory saved successfully!";
            }
            else
            {
                message = $"Inventory not captured successfull!";
            }
            var toast = Toast.Make(message, ToastDuration.Long, 30);
            await toast.Show();

            LoadInventory();

            // Optionally, clear the fields after saving
            NumberOfTraysBought = 0;
            NumberOfDamagedTrays = 0;
            NumberOfTraysSold = 0;
            Date = DateTime.Now;
        }
        public async void LoadInventory()
        {
            try
            {
                var inventory = await _inventoryService.GetInventory();

                InventoryRecords.Clear();
                foreach (var inventoryItem in inventory)
                {
                    InventoryRecords.Add(inventoryItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public int InventoryNumber
        {
            get => _inventoryNumber;
            set
            {
                _inventoryNumber = value;
                OnPropertyChanged(nameof(_inventoryNumber));
            }
        }
        public int NumberOfTraysBought
        {
            get => _numberOfTraysBought;
            set
            {
                _numberOfTraysBought = value;
                OnPropertyChanged(nameof(_numberOfTraysBought));
            }
        }
        public int NumberOfDamagedTrays
        {
            get => _numberOfDamagedTrays;
            set
            {
                _numberOfDamagedTrays = value;
                OnPropertyChanged(nameof(_numberOfDamagedTrays));
            }
        }
        public int NumberOfTraysSold
        {
            get => _numberOfTraysSold;
            set
            {
                _numberOfTraysSold = value;
                OnPropertyChanged(nameof(_numberOfTraysSold));
            }
        }
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(_date));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
