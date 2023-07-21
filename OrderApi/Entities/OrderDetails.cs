using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailsID { get; set; }
        public int OrderID { get; set; }

        [ForeignKey("OrderID")]
        public Order? Order { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product? Product { get; set; } = default!;

        public double? UnitPrice { get; init; }

        public long? Quantity { get; init; }

        public double? Discount { get; init; }
    }
}
