using CommunityToolkit.Maui.Alerts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Helpers;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Sales> SalesDetails { get; set; }
        private readonly ISalesService _salesService;
        private readonly IInventoryService _inventoryService;
        private int? _totalTraysSold;
        private decimal? _totalRevenue;
        private decimal? _totalProfitLoss;
        private int? _totalTraysLeft;
        private int? _totalTraysBroken;
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExportToPdfCommand { get; }
        public ICommand GenerateSalesReportCommand { get; }
        public  SalesViewModel(ISalesService salesService, IInventoryService inventoryService)
        {
            _salesService = salesService;
            _inventoryService = inventoryService;
            SalesDetails = new ObservableCollection<Sales>();
            //GenerateSalesReportCommand = new Command(GenerateReport);
            ExportToExcelCommand = new Command(ExportToExcelAsync);
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
        public async void ExportToExcelAsync()
        {
            // Set the LicenseContext for EPPlus
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            try
            {
                // Use EPPlus to create an Excel file
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

                    // Write totals to the next row
                    worksheet.Cells[row, 1].Value = "Total";
                    worksheet.Cells[row, 2].Value = TotalRevenue;
                    worksheet.Cells[row, 3].Value = TotalProfitLoss;
                    worksheet.Cells[row, 4].Value = TotalTraysLeft;
                    worksheet.Cells[row, 5].Value = TotalTraysBroken;

                    // Optionally, you can format the total row
                    using (var range = worksheet.Cells[row, 1, row, 5])
                    {
                        range.Style.Font.Bold = true; // Make the total row bold
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Set background color
                    }

                    // Ensure the directory exists
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var directoryPath = Path.Combine(documentsPath, "SalesReports");
                    Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

                    var filePath = Path.Combine(directoryPath, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} SalesReport.xlsx");

                    // Save the Excel file
                    var fileBytes = package.GetAsByteArray();
                    await File.WriteAllBytesAsync(filePath, fileBytes);

                    // Show success toast
                    var successMessage = $"Export successful! Sales report saved to {filePath}";
                    var toast = Toast.Make(successMessage, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                    await toast.Show();

                    await PreviewFile(filePath);
                }
            }
            catch (Exception ex)
            {
                // Show error toast with detailed message
                var errorMessage = $"Export failed: {ex.Message}";
                var toast = Toast.Make(errorMessage, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                await toast.Show();
            }
        }
        public async void ExportToPdf()
        {
            try
            {
                // Use a library like iTextSharp to create a PDF file
                using (var stream = new MemoryStream())
                {
                    var document = new Document();
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    document.Add(new Paragraph("Sales Report"));
                    document.Add(new Paragraph(" ")); // Add some space



                    // Add sales details to the PDF and calculate totals
                    foreach (var sale in SalesDetails)
                    {
                        document.Add(new Paragraph($"Batch: {sale.Id}, Trays Sold: {sale.NumberOfTraysSold}, Revenue: {sale.Revenue}, Profit/Loss: {sale.ProfitLoss}, Trays Left: {sale.NumberOfTraysLeft}, Trays Broken: {sale.NumberOfTraysBroken}"));

                    }

                    // Add totals to the PDF
                    document.Add(new Paragraph(" ")); // Add some space
                    document.Add(new Paragraph("Total:"));
                    document.Add(new Paragraph($"Total Trays Sold: {TotalTraysSold}"));
                    document.Add(new Paragraph($"Total Revenue: {TotalRevenue}"));
                    document.Add(new Paragraph($"Total Profit/Loss: {TotalProfitLoss}"));
                    document.Add(new Paragraph($"Total Trays Left: {TotalTraysLeft}"));
                    document.Add(new Paragraph($"Total Trays Broken: {TotalTraysBroken}"));

                    document.Close();

                    // Save the PDF file to a location
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} SalesReport.pdf");
                    File.WriteAllBytes(filePath, stream.ToArray());

                    // Show success toast
                    var toast = Toast.Make($"Export Successful! Sales report exported to {filePath}", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                    await toast.Show();

                   await PreviewFile(filePath);
                }
            }
            catch (Exception ex)
            {
                var toast = Toast.Make($"Export failed: {ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                await toast.Show();
            }
        }
        public async Task PreviewFile(string filePath)
        {
            // Open the PDF file for preview using Launcher
            var file = new ReadOnlyFile(filePath);
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = file
            });
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
