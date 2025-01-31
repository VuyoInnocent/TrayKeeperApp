using CommunityToolkit.Maui.Alerts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TrayKeeper.BL;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.ViewModel
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private readonly ISalesService _salesService;
        private readonly IInventoryService _inventoryService;

        public ObservableCollection<Inventory> SalesDetails { get; set; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExportToPdfCommand { get; }
        public ICommand GenerateSalesReportCommand { get; }

        public  SalesViewModel(ISalesService salesService, IInventoryService inventoryService)
        {
            _salesService = salesService;
            _inventoryService = inventoryService;
            try
            {
                SalesDetails = new ObservableCollection<Inventory>();
                GenerateSalesReportCommand = new Command(GenerateReport);
                //ExportToExcelCommand = new Command(ExportToExcel);
                //ExportToPdfCommand = new Command(ExportToPdf);
                LoadSalesDetails();
            }
            catch (Exception ex)
            {
                var toast = Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
                toast.Show();
            }

        }

        public async void GenerateReport()
        {
            await _salesService.GenerateSalesReport();
            LoadSalesDetails();
        }

        public async void LoadSalesDetails()
        {
            var inventories = await _salesService.GetSales();
            SalesDetails.Clear();
            foreach (var inventory in inventories)
            {
                SalesDetails.Add(inventory);
            }
        }

        //private async void ExportToExcel()
        //{
        //    // Use a library like EPPlus to create an Excel file
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Sales Report");
        //        worksheet.Cells[1, 1].Value = "Number of Trays Sold";
        //        worksheet.Cells[1, 2].Value = "Revenue";
        //        worksheet.Cells[1, 3].Value = "Profit/Loss";
        //        worksheet.Cells[1, 4].Value = "Number of Trays Left";
        //        worksheet.Cells[1, 5].Value = "Number of Trays Broken";

        //        int row = 2;
        //        foreach (var sale in SalesDetails)
        //        {
        //            worksheet.Cells[row, 1].Value = sale.NumberOfTraysSold;
        //            worksheet.Cells[row, 2].Value = sale.Revenue;
        //            worksheet.Cells[row, 3].Value = sale.ProfitLoss;
        //            worksheet.Cells[row, 4].Value = sale.NumberOfTraysLeft;
        //            worksheet.Cells[row, 5].Value = sale.NumberOfTraysBroken;
        //            row++;
        //        }

        //        // Save the Excel file to a location
        //        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SalesReport.xlsx");
        //        File.WriteAllBytes(filePath, package.GetAsByteArray());
        //        var toast = Toast.Make($"Export Successful Sales report exported to {filePath}", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
        //        await toast.Show();

        //    }
        //}

        //private async void ExportToPdf()
        //{
        //    // Use a library like iTextSharp to create a PDF file
        //    using (var stream = new MemoryStream())
        //    {
        //        var document = new iTextSharp.text.Document();
        //        PdfWriter.GetInstance(document, stream);
        //        document.Open();

        //        document.Add(new Paragraph("Sales Report"));
        //        document.Add(new Paragraph(" ")); // Add some space

        //        foreach (var sale in SalesDetails)
        //        {
        //            document.Add(new Paragraph($"Trays Sold: {sale.NumberOfTraysSold}, Revenue: {sale.Revenue}, Profit/Loss: {sale.ProfitLoss}, Trays Left: {sale.NumberOfTraysLeft}, Trays Broken: {sale.NumberOfTraysBroken}"));
        //        }

        //        document.Close();

        //        // Save the PDF file to a location
        //        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SalesReport.pdf");
        //        File.WriteAllBytes(filePath, stream.ToArray());
        //        var toast = Toast.Make($"Export Successful Sales report exported to {filePath}", CommunityToolkit.Maui.Core.ToastDuration.Long, 30);
        //        await toast.Show();
        //    }
        //}

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
