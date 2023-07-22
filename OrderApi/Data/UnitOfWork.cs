using AutoMapper;
using OrderApi.Repositories;

namespace OrderApi.Data
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly AppOrderContext context;
        private OrderRepository orderRepository;
        private IMapper mapper;
        public UnitOfWork(AppOrderContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                if(orderRepository == null)
                {
                    orderRepository = new OrderRepository(context,mapper);
                }
                return orderRepository;
            }
        }
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
