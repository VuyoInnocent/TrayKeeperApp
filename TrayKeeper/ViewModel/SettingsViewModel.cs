using CommunityToolkit.Maui.Alerts;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.Helpers;

namespace TrayKeeper.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private decimal _traySellingPrice;
        private decimal _trayCostPrice;

        public decimal TraySellingPrice
        {
            get => _traySellingPrice;
            set
            {
                if (_traySellingPrice != value)
                {
                    _traySellingPrice = value;
                    OnPropertyChanged(nameof(TraySellingPrice));
                }
            }
        }

        public decimal TrayCostPrice
        {
            get => _trayCostPrice;
            set
            {
                if (_trayCostPrice != value)
                {
                    _trayCostPrice = value;
                    OnPropertyChanged(nameof(TrayCostPrice));
                }
            }
        }

        public ICommand SaveCommand { get; }

        public SettingsViewModel()
        {
            SaveCommand = new Command(SaveSettings);
            // Initialize with current values
            TraySellingPrice = Constants.TraySellingPrice; 
            TrayCostPrice = Constants.TrayCostPrice;

   
        }

        private async void SaveSettings()
        {
            // Save the updated values to your settings storage
            Constants.TraySellingPrice = TraySellingPrice;
            Constants.TrayCostPrice = TrayCostPrice;

            var toast2 = Toast.Make("Settings saved successfully!", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
            await toast2.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
