using MediatR;
using OrderApi.CQRS.Data;
using OrderApi.CQRS.Models;
using OrderApi.CQRS.Resources.Commands;

namespace OrderApi.CQRS.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly ProductContext _dbContext;

        public CreateProductCommandHandler(ProductContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
            };

            _dbContext.Products.Add(product);
            return product;
        }
    }
}
