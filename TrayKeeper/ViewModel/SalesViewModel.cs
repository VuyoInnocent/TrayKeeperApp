using CommunityToolkit.Maui.Alerts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Sales> SalesDetails { get; set; }
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;
        private int? _totalTraysSold;
        private decimal? _totalRevenue;
        private decimal? _totalProfitLoss;
        private int? _totalTraysLeft;
        private int? _totalTraysBroken;  
        public ICommand ExportSalesToPdfCommand { get; }  
        public ICommand ExportOrdersToPdfCommand { get; }
        public ICommand ExportSalesToExcelCommand { get; }
        public ICommand ExportOrdersToExcelCommand { get; }
        public ICommand ImportOrdersFromExcelCommand { get; }
        public  SalesViewModel(ISalesService salesService,
            IInventoryService inventoryService,IOrderService orderService)
        {
            _orderService = orderService;
            _inventoryService = inventoryService;
            SalesDetails = new ObservableCollection<Sales>();
            ExportSalesToPdfCommand = new Command(ExportSalesToPdfCommandAsync); 
            ExportOrdersToPdfCommand = new Command(ExportOrdersToPdfCommandAsync);
            ExportSalesToExcelCommand = new Command(ExportSalesToExcelCommandAsync);
            ExportOrdersToExcelCommand = new Command(ExportOrdersToExcelCommandAsync);
            ImportOrdersFromExcelCommand = new Command(ImportOrdersFromExcelCommandAsync);
            LoadSalesDetails();
        }

        public async void ImportOrdersFromExcelCommandAsync()
        {
            try
            {
                var fileResult = await FilePicker.Default.PickAsync();
                if (fileResult == null) return;

                if (!fileResult.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    await ShowToast("Please select an Excel file (.xlsx)");
                    return;
                }

                // Get the stream from the FileResult
                using var stream = await fileResult.OpenReadAsync();

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets["Orders"];
                    if (worksheet == null)
                    {
                        await ShowToast("The Excel file doesn't contain an 'Orders' sheet");
                        return;
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    if (rowCount < 2)
                    {
                        await ShowToast("No data found in the Excel file");
                        return;
                    }

                    var importedOrders = new List<Orders>();
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var order = new Orders
                            {
                                Id = worksheet.Cells[row, 1].GetValue<int>(),
                                BatchNumber = worksheet.Cells[row, 2].GetValue<int>(),
                                ClientName = worksheet.Cells[row, 3].GetValue<string>(),
                                Cellphone = worksheet.Cells[row, 4].GetValue<string>(),
                                Location = worksheet.Cells[row, 5].GetValue<string>(),
                                NumberTraysBought = worksheet.Cells[row, 6].GetValue<int>(),
                                IsPaid = worksheet.Cells[row, 7].GetValue<string>().ToLower().Equals("paid") ? true : false,
                                IsCollected = worksheet.Cells[row, 8].GetValue<string>().ToLower().Equals("yes") ? true : false,
                                DateOrdered = DateTime.Parse(worksheet.Cells[row, 9].GetValue<string>())
                            };
                            importedOrders.Add(order);
                        }
                        catch (Exception ex)
                        {
                            await ShowToast($"Error reading row {row}: {ex.Message}");
                            continue;
                        }
                    }

                    // Save to database
                    var successCount = await _orderService.ImportOrders(importedOrders);
                    if (successCount > 0)
                    {
                        await ShowToast($"Successfully imported {successCount} of {importedOrders.Count} orders");
                    }

                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Import failed: {ex.Message}");
            }
        }
        public async void ExportOrdersToExcelCommandAsync()
        {
            // Set the LicenseContext for EPPlus
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            try
            {
                var ordersList = await _orderService.GetOrders();

                if (!ordersList.Any())
                {
                    await ShowToast("No Data available to  Export");
                    return;
                }

                // Use EPPlus to create an Excel file
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Orders Report");

                    // Add headers for all columns
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Batch Number";
                    worksheet.Cells[1, 3].Value = "Client Name";
                    worksheet.Cells[1, 4].Value = "Cellphone";
                    worksheet.Cells[1, 5].Value = "Location";
                    worksheet.Cells[1, 6].Value = "Trays Bought";
                    worksheet.Cells[1, 7].Value = "Paid?";
                    worksheet.Cells[1, 8].Value = "Collected?";
                    worksheet.Cells[1, 9].Value = "Date Ordered";

                    // Format headers
                    using (var range = worksheet.Cells[1, 1, 1, 9])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    int row = 2;
                    foreach (var order in ordersList)
                    {
                        worksheet.Cells[row, 1].Value = order.Id;
                        worksheet.Cells[row, 2].Value = order.BatchNumber;
                        worksheet.Cells[row, 3].Value = order.ClientName;
                        worksheet.Cells[row, 4].Value = order.Cellphone;
                        worksheet.Cells[row, 5].Value = order.Location;
                        worksheet.Cells[row, 6].Value = order.NumberTraysBought;
                        worksheet.Cells[row, 7].Value = order.IsPaid ? "Paid" : "Not Paid";
                        worksheet.Cells[row, 8].Value = order.IsCollected ? "Yes" : "No";
                        worksheet.Cells[row, 9].Value = order.DateOrdered;

                        // Format date column
                        worksheet.Cells[row, 9].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

                        row++;
                    }

                    // Ensure the directory exists
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var directoryPath = Path.Combine(documentsPath, "OrdersReports");
                    Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

                    var filePath = Path.Combine(directoryPath, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} OrdersReports.xlsx");

                    // Save the Excel file
                    var fileBytes = package.GetAsByteArray();
                    await File.WriteAllBytesAsync(filePath, fileBytes);

                    // Show success toast
                    await ShowToast($"Export successful! Orders report saved to {filePath}");

                    await PreviewFile(filePath);
                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Export failed: {ex.Message}");
            }
        }
        public async void LoadSalesDetails()
        {
            var inventories = await _inventoryService.GetInventory();
            var orders = await _orderService.GetOrders();
            SalesDetails.Clear();

            foreach (var inventory in inventories)
            {
                var paidOrders = orders
                    .Where(order => order.BatchNumber == inventory.Id && order.IsPaid)
                    .Select(order => order.NumberTraysBought)
                    .Sum();

                var batchPrice =  inventory.NumberOfTraysBought * inventory?.TrayCostPrice;
                var traysleft = (inventory?.NumberOfTraysBought + inventory?.NumberOfTraysSold) - inventory?.NumberOfTraysSold;
                decimal? eggsSold = paidOrders * inventory?.TraySellingPrice;
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
        public async void ExportSalesToExcelCommandAsync()
        {
            // Set the LicenseContext for EPPlus
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            try
            {
                if (!SalesDetails.Any())
                {
                    await ShowToast("No Data available to  Export");
                    return;
                }
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

                    await ShowToast($"Export successful! Sales report saved to {filePath}");
                 
                    await PreviewFile(filePath);
                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Export failed: {ex.Message}");
            }
        }
        public async void ExportSalesToPdfCommandAsync()
        {
            try
            {
                if (!SalesDetails.Any())
                {
                    await ShowToast("No Data available to  Export");
                    return;
                }
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

                    await ShowToast($"Export Successful! Sales report exported to {filePath}");
                
                    await PreviewFile(filePath);
                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Export failed: {ex.Message}");
            }
        }
        public async void ExportOrdersToPdfCommandAsync()
        {
            try
            {
                var ordersList = await _orderService.GetOrders();

                if (!ordersList.Any())
                {
                    await ShowToast("No orders available to export");
                    return;
                }

                // Use iTextSharp to create a PDF file
                using (var stream = new MemoryStream())
                {
                    var document = new Document();
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Add title and date
                    document.Add(new Paragraph("ORDERS REPORT"));
                    document.Add(new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"));
                    document.Add(new Paragraph(" ")); // Add space

                    // Create a table for the orders data
                    PdfPTable table = new PdfPTable(9); // 9 columns matching your Excel export
                    table.WidthPercentage = 100;

                    // Add headers
                    table.AddCell(new Phrase("ID"));
                    table.AddCell(new Phrase("Batch #"));
                    table.AddCell(new Phrase("Client"));
                    table.AddCell(new Phrase("Cellphone"));
                    table.AddCell(new Phrase("Location"));
                    table.AddCell(new Phrase("Trays"));
                    table.AddCell(new Phrase("Paid?"));
                    table.AddCell(new Phrase("Collected?"));
                    table.AddCell(new Phrase("Date"));

                    // Add order data
                    foreach (var order in ordersList)
                    {
                        table.AddCell(order.Id.ToString());
                        table.AddCell(order.BatchNumber.ToString());
                        table.AddCell(order.ClientName);
                        table.AddCell(order.Cellphone);
                        table.AddCell(order.Location);
                        table.AddCell(order.NumberTraysBought.ToString());
                        table.AddCell(order.IsPaid ? "Yes" : "No");
                        table.AddCell(order.IsCollected ? "Yes" : "No");
                        table.AddCell(order.DateOrdered.ToString("yyyy-MM-dd"));
                    }

                    document.Add(table);

                    // Add summary statistics
                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph($"Total Orders: {ordersList.Count()}"));
                    document.Add(new Paragraph($"Total Trays: {ordersList.Sum(o => o.NumberTraysBought)}"));
                    document.Add(new Paragraph($"Paid Orders: {ordersList.Count(o => o.IsPaid)}"));
                    document.Add(new Paragraph($"Collected Orders: {ordersList.Count(o => o.IsCollected)}"));

                    document.Close();

                    // Save the PDF file
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var directoryPath = Path.Combine(documentsPath, "OrdersReports");
                    Directory.CreateDirectory(directoryPath);

                    var filePath = Path.Combine(directoryPath, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} OrdersReport.pdf");
                    File.WriteAllBytes(filePath, stream.ToArray());

                    await ShowToast($"Export successful! Orders report saved to {filePath}");
                    await PreviewFile(filePath);
                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Export failed: {ex.Message}");
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
        private async Task ShowToast(string message)
        {
            var toast = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
            await toast.Show();
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
