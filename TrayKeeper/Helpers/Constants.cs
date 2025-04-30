namespace TrayKeeper.Helpers
{
    public static class Constants
    {
        public static string DatabaseFilename = "TrayKepeer.db3";

        public const SQLite.SQLiteOpenFlags Flags =
       // open the database in read/write mode
       SQLite.SQLiteOpenFlags.ReadWrite |
       // create the database if it doesn't exist
       SQLite.SQLiteOpenFlags.Create |
       // enable multi-threaded database access
       SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static decimal TraySellingPrice { get; set; } = 70;
        public static decimal TrayCostPrice { get; set; } = 47.5m;

        public static string createUsersTable =
@"CREATE TABLE [User] (
Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
Username text NOT NULL,
PasswordHash text NOT NULL );";

        public static string createOrdersTable =
@"CREATE TABLE [Orders] (
Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
BatchNumber INTEGER NOT NULL,
ClientName TEXT NOT NULL,
Cellphone TEXT NOT NULL,
Location TEXT NOT NULL,
NumberTraysBought INTEGER NOT NULL,
IsPaid BOOLEAN NOT NULL,
IsCollected BOOLEAN NOT NULL,
DateOrdered DATETIME NOT NULL );";

        public static string createSalesTable =
@"CREATE TABLE [Sales] (
Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
NumberOfTraysSold INTEGER,
Revenue DECIMAL(18, 2) NOT NULL,
ProfitLoss DECIMAL(18, 2) NOT NULL,
NumberOfTraysLeft INTEGER,
NumberOfTraysBroken INTEGER,
Date DATETIME );";
         
        public static string createInventoryTable =
@"CREATE TABLE [Inventory] (
Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
InventoryNumber INTEGER NULL,
NumberOfTraysBought INTEGER NULL,
NumberOfDamagedTrays INTEGER NULL,
NumberOfTraysSold INTEGER NULL,
TraySellingPrice DECIMAL(18, 2) NOT NULL,
TrayCostPrice DECIMAL(18, 2) NOT NULL,
Date DATETIME NULL );";

    }
}
