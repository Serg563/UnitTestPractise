using AutoMapper;
using OrderApi.Data;
using OrderApi.DTO;
using OrderApi.Entities;
using OrderApi.Models;

namespace OrderApi.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OrderGetAllModel>> GetAllOrders()
        {
            var orders = await unitOfWork.OrderRepository.GetAllOrders();
            var mapped = mapper.Map<IEnumerable<OrderGetAllModel>>(orders);
            return mapped;
        }
        public async Task<Order> GetOrderById(int id)
        {
            var res = await unitOfWork.OrderRepository.GetOrderById(id);
            await unitOfWork.SaveAsync();
            return res;
        }

        public async Task DeleteOrder(int id)
        {
            await unitOfWork.OrderRepository.DeleteOrder(id);
            await unitOfWork.SaveAsync();
        }
        public async Task UpdateOrder(Order order)
        {
            unitOfWork.OrderRepository.UpdateOrder(order);
            await unitOfWork.SaveAsync();
        }

        public async Task AddOrder(AddOrderDTO order)
        {
            unitOfWork.OrderRepository.AddOrder(order);
            await unitOfWork.SaveAsync();
        }
    }
}
