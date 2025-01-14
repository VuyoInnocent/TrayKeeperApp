using TrayKeeper.Models;

namespace TrayKeeper.BL.Interfaces
{
    public  interface ISalesService
    {
        Task<int> GenerateSalesReport();
        Task<IEnumerable<Sales>> GetSales();
    }
}
