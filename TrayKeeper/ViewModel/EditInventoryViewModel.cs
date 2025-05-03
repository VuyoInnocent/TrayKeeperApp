using CommunityToolkit.Maui.Alerts;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class EditInventoryViewModel : INotifyPropertyChanged
    {
        private readonly IInventoryService _inventoryService;
        private Inventory _inventory;
        private Action _onInventoryUpdated;

        public EditInventoryViewModel(IInventoryService inventoryService,Inventory inventory, Action onInventoryUpdated)
        {
            SaveInventoryCommand = new Command(SaveInventory);
            _inventory = inventory;
            _onInventoryUpdated = onInventoryUpdated;
            _inventoryService = inventoryService;
        }
        private async void SaveInventory()
        {
            try
            {
                if (NumberOfDamagedTrays > NumberOfTraysBought)
                {
                    await ShowToast("Error,number of damanged cannot be greater than number of trays bought!");
                    return;
                }
                else
                {
                    _inventory.NumberOfTraysBought = NumberOfTraysBought - NumberOfDamagedTrays;
                    _inventory.NumberOfDamagedTrays = NumberOfDamagedTrays;
                }


                 var result = await _inventoryService.UpdateInventory(_inventory);

                if (result > 0)
                {
                    await ShowToast("Inventory updated successfully!");
                    _onInventoryUpdated?.Invoke(); // Trigger the event
                }
                else
                {
                    await ShowToast("Inventory updated not updated!");
                }

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await ShowToast($"Saving failed: {ex.Message}");
            }


           
        }
        private async Task ShowToast(string message)
        {
            var toast = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
            await toast.Show();
        }
        public int NumberOfTraysBought
        {
            get => _inventory.NumberOfTraysBought.GetValueOrDefault();
            set
            {
                _inventory.NumberOfTraysBought = value;
                OnPropertyChanged(nameof(NumberOfTraysBought));
            }
        }
        public int NumberOfDamagedTrays
        {
            get => _inventory.NumberOfDamagedTrays.GetValueOrDefault();
            set
            {
                _inventory.NumberOfDamagedTrays = value;
                OnPropertyChanged(nameof(NumberOfDamagedTrays));
            }
        }
        public int NumberOfTraysSold
        {
            get => _inventory.NumberOfTraysSold.GetValueOrDefault();
            set
            {
                _inventory.NumberOfTraysSold = value;
                OnPropertyChanged(nameof(NumberOfTraysSold));
            }
        }
        public DateTime Date
        {
            get => _inventory.Date.GetValueOrDefault();
            set
            {
                _inventory.Date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public ICommand SaveInventoryCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
