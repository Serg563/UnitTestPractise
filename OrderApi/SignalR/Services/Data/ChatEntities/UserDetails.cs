using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderApi.Controllers;

namespace TaskManagerApi.Entities
{
    public class UserDetails
    {
        [Key]
        public int Id { get; set; }
        public string? Photo {get;set;}
        public string? Position { get; set; }
        public string? Hobbies { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; } 
        public string? Team { get; set; }

        [ForeignKey("User")]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}