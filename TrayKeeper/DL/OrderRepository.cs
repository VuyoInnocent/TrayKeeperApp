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
    internal class OrderRepository : GenericRepository<Orders>, IOrderRepository
    {
        public OrderRepository(SQLiteAsyncConnection database) : base(database)
        {
        }
    }
}
