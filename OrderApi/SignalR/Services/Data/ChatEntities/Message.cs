using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderApi.Controllers;

namespace OrderApi.SignalR.Services.Data.ChatEntities
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDateTime { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }    

        public MessageGroup? Group { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
