using System.Data.Common;
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

        public async Task<int> ImportOrders(List<Orders> orders)
        {
            int successCount = 0;

            var allOrders = await _orderRepository.GetAllAsync();

            foreach (var order in orders)
            {
                try
                {
                    // Check if order exists (update) or needs to be inserted
                    var existingOrder = allOrders.Where(o => o.Id == order.Id).FirstOrDefault();

                    if (existingOrder != null)
                    {
                        // Update existing order
                        existingOrder.BatchNumber = order.BatchNumber;
                        existingOrder.ClientName = order.ClientName;
                        existingOrder.Cellphone = order.Cellphone;
                        existingOrder.Location = order.Location;
                        existingOrder.NumberTraysBought = order.NumberTraysBought;
                        existingOrder.IsPaid = order.IsPaid;
                        existingOrder.IsCollected = order.IsCollected;
                        existingOrder.DateOrdered = order.DateOrdered;

                        await _orderRepository.UpdateAsync(existingOrder);
                    }
                    else
                    {
                        var newOrder = new Orders
                        {
                            BatchNumber = order.BatchNumber,
                            ClientName = order.ClientName,
                            Cellphone = order.Cellphone,
                            Location = order.Location,
                            NumberTraysBought = order.NumberTraysBought,
                            IsPaid = order.IsPaid,
                            IsCollected = order.IsCollected,
                            DateOrdered = order.DateOrdered
                        };
                        await _orderRepository.InsertAsync(newOrder);
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    // Log error but continue with other orders
                   throw new Exception($"Error importing order ID {order.Id}: {ex.Message}");
                }
            }

            return successCount;
        }
    }
}
