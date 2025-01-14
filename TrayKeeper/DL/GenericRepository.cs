using SQLite;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Helpers;

namespace TrayKeeper.DL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private SQLiteAsyncConnection _database;

        public GenericRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task Init()
        {
            try
            {
                //var d = await DeleteTableIfExistsAsync();

                if (_database == null)
                {
                    _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                    var result = await CreateTableAsync();
                }

                if (await DoesTableExist())
                {
                   var result = await CreateTableAsync();
                }
         
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<int> CreateTableAsync()
        {
            var tableName = typeof(T).Name;
            var query = string.Empty;
            switch (tableName)
            {
                case "User":
                    query = Constants.createUsersTable;
                    break;
                case "Sales":
                    query = Constants.createSalesTable;
                    break;
                case "Inventory":
                    query = Constants.createInventoryTable;
                    break;
                case "Orders":
                    query = Constants.createOrdersTable;
                    break;
                default:
                    break;
            }

            return await _database.ExecuteAsync(query);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var tableName = typeof(T).Name;
                await Init();
                return await _database.Table<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty<T>();
        }
        public async Task<int> UpdateAsync(T item)
        {
            await Init();
            return await _database.UpdateAsync(item);
        }
        public async Task<int> InsertAsync(T item)
        {
            await Init();
            return await _database.InsertAsync(item);
        }
        public async Task<int> DeleteAsync(T item)
        {
            await Init();
            return await _database.DeleteAsync(item);
        }
        public async Task<bool> DoesTableExist()
        {
            var tableName = typeof(T).Name;
            var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            var result = await _database.ExecuteScalarAsync<string>(query);
            return string.IsNullOrEmpty(result);
        }
        public async Task<int> DeleteTableIfExistsAsync()
        {
            List<string> tablesNames = new List<string>
            {
                //"User",
                //"Sales",
                //"Order",
                "Inventory"
            };
            int count = 0;

            foreach (var tableName in tablesNames)
            {
                var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                var result = await _database.ExecuteScalarAsync<string>(query);

                // If the table exists, delete it
                if (!string.IsNullOrEmpty(result))
                {
                    var deleteQuery = $"DROP TABLE [{result}]";

                    try
                    {
                        Console.WriteLine($"Executing query: {deleteQuery}");
                        await _database.ExecuteAsync(deleteQuery); // Use ExecuteAsync or ExecuteNonQueryAsync
                        count++; // Increment count for each table dropped
                    }
                    catch (Exception ex)
                    {
                        // Log the exception (consider using a logging framework)
                        Console.WriteLine($"Error dropping table {result}: {ex.Message}");
                    }
                }
            }

            return count; // Table did not exist
        }
    }
}