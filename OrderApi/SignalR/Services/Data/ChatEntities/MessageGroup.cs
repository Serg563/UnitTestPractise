using System.ComponentModel.DataAnnotations;

namespace OrderApi.SignalR.Services.Data.ChatEntities
{
    public class MessageGroup
    {
        [Key]
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public ICollection<GroupMember> GroupMembers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
