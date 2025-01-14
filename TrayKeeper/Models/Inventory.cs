namespace TrayKeeper.Models
{
    public class Inventory
    {
        [SQLite.PrimaryKey]
        public int? Id { get; set; }
        public int? InventoryNumber { get; set; }
        public int? NumberOfTraysBought { get; set; }
        public int? NumberOfDamagedTrays { get; set; }
        public int? NumberOfTraysSold { get; set; }
        public DateTime? Date { get; set; }
    }
}
