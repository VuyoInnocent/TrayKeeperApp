using TrayKeeper.Models;

namespace TrayKeeper.BL.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetInventory();
        Task<int> AddInventory(Inventory inventory);
        Task<int> UpdateInventory(Inventory inventory);
    }
}
