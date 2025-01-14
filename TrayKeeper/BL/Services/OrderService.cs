using TrayKeeper.BL.Interfaces;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.BL
{
    public class OrderService : IOrderService
    {
   
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<int> AddOrder(Orders order)
        {
            return await _orderRepository.InsertAsync(order);
        }

        public async Task<IEnumerable<Orders>> GetOrders()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<int> UpdateOrder(Orders order)
        {
            return await _orderRepository.UpdateAsync(order);
        }
    }
}
