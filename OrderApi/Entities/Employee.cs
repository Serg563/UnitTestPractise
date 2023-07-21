using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class Employee
    {
        [Key]
        [Column("EmployeeID")]
        public int Id { get; set; }

        public string? FirstName { get; init; } = default!;

        public string? LastName { get; init; } = default!;

        public DateTime? BirthDate { get; init; } = default!;

        public string? Title { get; init; } = default!;

        public string? Country { get; init; } = default!;

        public string? City { get; init; } = default!;
    }
}
