using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class Customer
    {
        [Key]
        [Column("CustomerID")]
        public int Id { get; set; }

        [Column("CompanyName")]
        public string? CompanyName { get; init; } = default!;

        [Column("ContactName")]
        public string? ContactName { get; init; } = default!;

        [Column("ContactTitle")]
        public string? ContactTitle { get; init; } = default!;

        //public List<Order>? Orders { get; init; }
    }
}
