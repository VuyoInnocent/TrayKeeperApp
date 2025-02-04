using CommunityToolkit.Maui.Alerts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;
using Constants = TrayKeeper.Helpers.Constants;

namespace TrayKeeper.ViewModel
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private readonly ISalesService _salesService;
        private readonly IInventoryService _inventoryService;
        private int? _totalTraysSold;
        private decimal? _totalRevenue;
        private decimal? _totalProfitLoss;
        private int? _totalTraysLeft;
        private int? _totalTraysBroken;
        public ObservableCollection<Sales> SalesDetails { get; set; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExportToPdfCommand { get; }
        public ICommand GenerateSalesReportCommand { get; }
        public  SalesViewModel(ISalesService salesService, IInventoryService inventoryService)
        {
            _salesService = salesService;
            _inventoryService = inventoryService;
            SalesDetails = new ObservableCollection<Sales>();
            GenerateSalesReportCommand = new Command(GenerateReport);
            ExportToExcelCommand = new Command(ExportToExcel);
            ExportToPdfCommand = new Command(ExportToPdf);
            LoadSalesDetails();
        }
        public async void GenerateReport()
        {
            await _salesService.GenerateSalesReport();
            LoadSalesDetails();
        }
        public async void LoadSalesDetails()
        {
            var inventories = await _inventoryService.GetInventory();
            SalesDetails.Clear();
            foreach (var inventory in inventories)
            {
                var batchPrice =  inventory.NumberOfTraysBought * Constants.TrayCostPrice;
                var traysleft = (inventory?.NumberOfTraysBought + inventory?.NumberOfTraysSold) - inventory?.NumberOfTraysSold;
                decimal? eggsSold = inventory?.NumberOfTraysSold * Constants.TraySellingPrice;
                var profitloss = eggsSold - batchPrice;

                SalesDetails.Add(new Sales{
                    Id = inventory?.Id,
                    NumberOfTraysSold = inventory?.NumberOfTraysSold,
                    NumberOfTraysBroken = inventory?.NumberOfDamagedTrays,
                    NumberOfTraysLeft = traysleft,
                    Revenue = eggsSold.HasValue ==  true ? eggsSold.Value : 0,
                    ProfitLoss = profitloss.HasValue == true ? profitloss.Value : 0,
                });
            }

            TotalTraysSold = SalesDetails.Sum(s => s.NumberOfTraysSold);
            TotalRevenue = SalesDetails.Sum(s => s.Revenue);
            TotalProfitLoss = SalesDetails.Sum(s => s.ProfitLoss);
            TotalTraysLeft = SalesDetails.Sum(s => s.NumberOfTraysLeft);
            TotalTraysBroken = SalesDetails.Sum(s => s.NumberOfTraysBroken);
        }
        private async void ExportToExcel()
        {
            try
            {
                    // Use a library like EPPlus to create an Excel file
                    using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sales Report");
                    worksheet.Cells[1, 1].Value = "Number of Trays Sold";
                    worksheet.Cells[1, 2].Value = "Revenue";
                    worksheet.Cells[1, 3].Value = "Profit/Loss";
                    worksheet.Cells[1, 4].Value = "Number of Trays Left";
                    worksheet.Cells[1, 5].Value = "Number of Trays Broken";

                    int row = 2;
                    foreach (var sale in SalesDetails)
                    {
                        worksheet.Cells[row, 1].Value = sale.NumberOfTraysSold;
                        worksheet.Cells[row, 2].Value = sale.Revenue;
                        worksheet.Cells[row, 3].Value = sale.ProfitLoss;
                        worksheet.Cells[row, 4].Value = sale.NumberOfTraysLeft;
                        worksheet.Cells[row, 5].Value = sale.NumberOfTraysBroken;
                        row++;
                    }

                    // Save the Excel file to a location
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SalesReport.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    var toast = Toast.Make($"Export Successful Sales report exported to {filePath}", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                    await toast.Show();

                }

            }
            catch (Exception ex)
            {
                var toast = Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                await toast.Show();
            }
        }
        private async void ExportToPdf()
        {
            try {
                // Use a library like iTextSharp to create a PDF file
                using (var stream = new MemoryStream())
                {
                    var document = new iTextSharp.text.Document();
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    document.Add(new Paragraph("Sales Report"));
                    document.Add(new Paragraph(" ")); // Add some space

                    foreach (var sale in SalesDetails)
                    {
                        document.Add(new Paragraph($"Trays Sold: {sale.NumberOfTraysSold}, Revenue: {sale.Revenue}, Profit/Loss: {sale.ProfitLoss}, Trays Left: {sale.NumberOfTraysLeft}, Trays Broken: {sale.NumberOfTraysBroken}"));
                    }

                    document.Close();

                    // Save the PDF file to a location
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SalesReport.pdf");
                    File.WriteAllBytes(filePath, stream.ToArray());
                    var toast = Toast.Make($"Export Successful Sales report exported to {filePath}", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                    await toast.Show();
                }
            }
            catch (Exception ex)
            {
                var toast = Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
        await toast.Show();
    }
}
        public int? TotalTraysSold
        {
            get => _totalTraysSold;
            set
            {
                _totalTraysSold = value;
                OnPropertyChanged(nameof(TotalTraysSold));
            }
        }
        public decimal? TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                _totalRevenue = value;
                OnPropertyChanged(nameof(TotalRevenue));
            }
        }
        public decimal? TotalProfitLoss
        {
            get => _totalProfitLoss;
            set
            {
                _totalProfitLoss = value;
                OnPropertyChanged(nameof(TotalProfitLoss));
            }
        }
        public int? TotalTraysLeft
        {
            get => _totalTraysLeft;
            set
            {
                _totalTraysLeft = value;
                OnPropertyChanged(nameof(TotalTraysLeft));
            }
        }
        public int? TotalTraysBroken
        {
            get => _totalTraysBroken;
            set
            {
                _totalTraysBroken = value;
                OnPropertyChanged(nameof(TotalTraysBroken));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
