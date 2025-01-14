using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.DL
{
    public class LogInRepository : GenericRepository<User>, ILogInRepository
    {
        public LogInRepository(SQLiteAsyncConnection database) : base(database)
        {
        }

    }
}
