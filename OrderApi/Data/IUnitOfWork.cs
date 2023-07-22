using OrderApi.Repositories;

namespace OrderApi.Data
{
    public interface IUnitOfWork
    {
        IOrderRepository OrderRepository { get; }

        Task SaveAsync();
    }
}
