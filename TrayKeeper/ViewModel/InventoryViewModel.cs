using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Helpers;
using TrayKeeper.Models;
using TrayKeeper.Views;

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
        public ICommand EditInventoryCommand { get; }
        public InventoryViewModel(IInventoryService inventoryService)
        {
            InventoryRecords = new ObservableCollection<Inventory>();
            SaveInventoryCommand = new Command(SaveInventory);
            EditInventoryCommand = new Command<Inventory>(OnEditInventory);
            _inventoryService = inventoryService;
            Date = DateTime.Now;

            LoadInventory();
        }
        private async void SaveInventory()
        {
            try
            {
                var inventory = new Inventory
                {
                    NumberOfTraysBought = NumberOfTraysBought,
                    NumberOfDamagedTrays = NumberOfDamagedTrays,
                    NumberOfTraysSold = NumberOfTraysSold,
                    TrayCostPrice = Constants.TrayCostPrice,
                    TraySellingPrice = Constants.TraySellingPrice,
                    Date = Date
                };

                if(string.IsNullOrWhiteSpace(NumberOfTraysBought+"") || NumberOfTraysBought <= 0||
                 string.IsNullOrWhiteSpace(NumberOfDamagedTrays + "") ||
                 string.IsNullOrWhiteSpace(NumberOfTraysSold + "") ||
                 string.IsNullOrWhiteSpace(Date + ""))
                {

                    await ShowToast("Enter all values");
           
                    return;
                }

                var result = await _inventoryService.AddInventory(inventory);

                if (result > 0)
                {
                    await ShowToast("Inventory saved successfully!");
                    clear();
                }
                else
                {
                    await ShowToast($"Inventory not captured successfull!");
                }
                LoadInventory();

            }
            catch (Exception ex)
            {
                await ShowToast($"Saving Inventory failed: {ex.Message}");
            }
           

        }
        private async void OnEditInventory(Inventory inventory)
        {
            if (inventory == null) return;

            var editPage = new EditInventoryPage(_inventoryService,inventory, LoadInventory);

            await Application.Current.MainPage.Navigation.PushAsync(editPage);
        }
        public async void LoadInventory()
        {
            try
            {
                var inventory = await _inventoryService.GetInventory();

                InventoryRecords.Clear();
                foreach (var inventoryItem in inventory.Where(x => x?.NumberOfTraysBought > 0))
                {
                    InventoryRecords.Add(inventoryItem);
                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Loading failed: {ex.Message}");
            }
        }

        private async Task ShowToast(string message)
        {
            var toast = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
            await toast.Show();
        }
        public void clear()
        {
            InventoryNumber = 0;
            NumberOfTraysBought = 0;
            NumberOfDamagedTrays = 0;
            NumberOfTraysSold = 0;
            Date = DateTime.Now;
        }
        public int InventoryNumber
        {
            get => _inventoryNumber;
            set
            {
                _inventoryNumber = value;
                OnPropertyChanged(nameof(InventoryNumber));
            }
        }
        public int NumberOfTraysBought
        {
            get => _numberOfTraysBought;
            set
            {
                _numberOfTraysBought = value;
                OnPropertyChanged(nameof(NumberOfTraysBought));
            }
        }
        public int NumberOfDamagedTrays
        {
            get => _numberOfDamagedTrays;
            set
            {
                _numberOfDamagedTrays = value;
                OnPropertyChanged(nameof(NumberOfDamagedTrays));
            }
        }
        public int NumberOfTraysSold
        {
            get => _numberOfTraysSold;
            set
            {
                _numberOfTraysSold = value;
                OnPropertyChanged(nameof(NumberOfTraysSold));
            }
        }
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
