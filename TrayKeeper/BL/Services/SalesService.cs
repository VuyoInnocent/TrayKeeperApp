using TrayKeeper.BL.Interfaces;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Helpers;
using TrayKeeper.Models;

namespace TrayKeeper.BL
{
    public class SalesService :  ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public SalesService(ISalesRepository salesRepository, 
                            IOrderRepository orderRepository, 
                            IInventoryRepository inventoryRepository)
        {
            _salesRepository = salesRepository;
            _orderRepository = orderRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<int> GenerateSalesReport()
        {
            var sales = new Sales
            {
                NumberOfTraysSold = await GetTotalTraysSold(),
                Revenue = await GetTotalRevenue(),
                ProfitLoss = await GetProfitLoss(),
                NumberOfTraysLeft = await GetTotalTraysLeft(),
                NumberOfTraysBroken = await GetTotalTraysBroken()
            };
            
            return await _salesRepository.InsertAsync(sales);
        }
        private async Task<int?> GetTotalTraysSold()
        {
            var results = await  GetSales();
            return results.Sum(o => o.NumberOfTraysSold);
        }
        private async Task<decimal> GetTotalRevenue()
        {
            var results = await _orderRepository.GetAllAsync();
            return results.Sum(o => o.NumberTraysBought * Constants.TraySellingPrice);
        }
        private async Task<decimal> GetProfitLoss()
        {
            var totalRevenue = await GetTotalRevenue();
            var results = await _inventoryRepository.GetAllAsync();
            var totalCost = results.Sum(i => i.NumberOfTraysBought * Constants.TrayCostPrice);
            var profitLoss = totalRevenue - totalCost;
            return profitLoss.GetValueOrDefault();
        }
        private async Task<int?> GetTotalTraysLeft()
        {
            var invetoryResults = await _inventoryRepository.GetAllAsync();
            var orderresults = await _orderRepository.GetAllAsync();
            return invetoryResults.Sum(i => i.NumberOfTraysBought - i.NumberOfDamagedTrays) - orderresults.Sum(o => o.NumberTraysBought);
        }
        private async Task<int> GetTotalTraysBroken()
        {
            var result = await _inventoryRepository.GetAllAsync();
            return result.Sum(i => i.NumberOfDamagedTrays.GetValueOrDefault());

        }
        public async Task<IEnumerable<Sales>> GetSales()
        {
            return await _salesRepository.GetAllAsync();
        }
    }
}
