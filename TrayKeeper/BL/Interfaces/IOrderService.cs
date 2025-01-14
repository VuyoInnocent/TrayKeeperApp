using TrayKeeper.Models;

namespace TrayKeeper.BL.Interfaces
{
    public interface IOrderService 
    {
        Task<int> AddOrder(Orders order);
        Task<int> UpdateOrder(Orders order);
        Task<IEnumerable<Orders>> GetOrders();
    }
}
