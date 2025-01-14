using TrayKeeper.BL.Interfaces;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.BL
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<int> AddInventory(Inventory inventory)
        {
            var countInventory = await GetInventory();

            inventory.InventoryNumber = countInventory.Count()+1;//Generate New Batch Number
            return await _inventoryRepository.InsertAsync(inventory); 
        }

        public async Task<IEnumerable<Inventory>> GetInventory()
        {
            return await _inventoryRepository.GetAllAsync();
        }
        public async Task<int> UpdateInventory(Inventory inventory)
        {
            return await _inventoryRepository.UpdateAsync(inventory);
        }
    }
}
