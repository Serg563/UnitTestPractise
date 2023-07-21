using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class Category
    {
        [Key]
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Column("CategoryName")]
        public string? CategoryName { get; set; }

        [Column("Description")]
        public string? Description { get; set; }
    }
}
