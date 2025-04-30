namespace TrayKeeper.Models
{
    public class Orders
    {
        [SQLite.PrimaryKey]
        public int? Id { get; set; }
        public int? BatchNumber { get; set; }
        public  string ClientName { get; set; }
        public  string Cellphone { get; set; }
        public  string Location { get; set; }
        public int NumberTraysBought { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCollected { get; set; }
        public DateTime DateOrdered { get; set; }

        public string FormattedDetails
        {
            get
            {
                return $"#: {NumberTraysBought} : Date: {DateOrdered.ToShortDateString()} \n" +
                       $" {(IsPaid ? "Paid" : "Not Paid")} : Collected: {(IsCollected ? "Yes" : "No")}";
            }
        }
    }
}
