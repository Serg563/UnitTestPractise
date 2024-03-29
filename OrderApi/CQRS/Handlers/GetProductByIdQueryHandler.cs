using MediatR;
using OrderApi.CQRS.Data;
using OrderApi.CQRS.Models;
using OrderApi.CQRS.Resources.Queries;

namespace OrderApi.CQRS.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
    {
        private readonly ProductContext _context;
        public GetProductByIdQueryHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) =>
            _context.Products.FirstOrDefault(x => x.Id == request.Id);
    }
}
