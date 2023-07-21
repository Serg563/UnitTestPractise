using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int Id { get; set; }

        public string? ProductName { get; init; } = default!;

        public int? SupplierID { get; init; }

        [ForeignKey("SupplierID")]
        public Supplier? SupplierSupplier { get; init; }

        public int? CategoryId { get; init; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; init; } = default!;
    }
}
