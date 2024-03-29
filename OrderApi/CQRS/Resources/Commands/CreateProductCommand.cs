using MediatR;
using OrderApi.CQRS.Models;

namespace OrderApi.CQRS.Resources.Commands
{
    public class CreateProductCommand : IRequest<Product>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool Active { get; set; } = true;
        public decimal Price { get; set; }
    }
}
