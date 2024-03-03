using Microsoft.EntityFrameworkCore;
using OrderApi.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.SignalR.Services.Data.ChatEntities
{
    
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }
        public MessageGroup Group { get; set; }
        public DateTime JoinedDateTime { get; set; }

    }
}
