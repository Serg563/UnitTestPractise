using OrderApi.Entities;
using OrderApi.Models;

namespace OrderApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderGetAllModel>> GetAllOrders();

        Task<Order> GetOrderById(int id);

        
    }
}
