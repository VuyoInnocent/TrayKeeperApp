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
    public class SaleRepository : GenericRepository<Sales>, ISalesRepository
    {
        public SaleRepository(SQLiteAsyncConnection database) : base(database)
        {
        }
    }
}
