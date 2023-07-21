using OrderApi.DTO;
using OrderApi.Entities;

namespace OrderApi.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        void AddOrder(AddOrderDTO order);

        void UpdateOrder(Order order);

        Task<int> DeleteOrder(int id);

        Task<Order> GetOrderById(int id);

    }
}
