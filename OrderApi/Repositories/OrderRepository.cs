using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.DTO;
using OrderApi.Entities;

namespace OrderApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppOrderContext context;
        private readonly IMapper mapper;
        public OrderRepository(AppOrderContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddOrder(AddOrderDTO order)
        {
            if (order != null)
            {
                var mapped = mapper.Map<Order>(order);
                mapped.OrderDate = DateTime.Now;
                mapped.RequiredDate = DateTime.Now.AddDays(1);
                context.AddAsync(mapped);
                context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteOrder(int id)
        {
            var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return order.Id;
            }
            return 0;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await context.Orders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await context.Orders.ToListAsync();
        }

        public void UpdateOrder(Order order)
        {
             context.Update(order);
             context.SaveChanges();
        }
    }
}
