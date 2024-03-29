using MediatR;
using OrderApi.CQRS.Models;

namespace OrderApi.CQRS.Resources.Queries
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public int Id { get; set; }
    }
}
