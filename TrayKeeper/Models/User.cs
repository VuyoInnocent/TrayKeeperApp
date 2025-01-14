namespace TrayKeeper.Models
{

    public class User
    {
        [SQLite.PrimaryKey]
        public int? Id { get; set; }
        public  string Username { get; set; }
        public  string PasswordHash { get; set; }

    }
}
