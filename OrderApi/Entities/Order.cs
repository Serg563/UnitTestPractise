using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class Order
    {
        [Key]
        [Column("OrderID")]
        public int Id { get; set; }

        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; } = default!;

        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public Employee? Employee { get; set; } = default!;

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int ShipVia { get; set; }

        [ForeignKey("ShipVia")]
        public Shipper? Shipper { get; set; } = default!;

        public double? Freight { get; set; }

        public string? ShipName { get; set; } = default!;

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
