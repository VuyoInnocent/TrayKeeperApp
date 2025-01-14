using SQLite;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.DL
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(SQLiteAsyncConnection database) : base(database)
        {
        }
    }
}
