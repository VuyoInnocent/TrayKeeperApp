using System.ComponentModel.DataAnnotations;

namespace TrayKeeper.Models
{
    public class Sales
    {
        [SQLite.PrimaryKey]
        public int? Id { get; set; }
        public int? NumberOfTraysSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal ProfitLoss { get; set; }
        public int? NumberOfTraysLeft { get; set; }
        public int? NumberOfTraysBroken { get; set; }
        public DateTime? Date { get; set; }
    }
}
