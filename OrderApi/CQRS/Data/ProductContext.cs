using OrderApi.CQRS.Models;

namespace OrderApi.CQRS.Data
{
    public class ProductContext
    {
        public List<Product> Products;
        public ProductContext()
        {
            Products = new();
        }
    }
}
