using System.ComponentModel.DataAnnotations;
using OrderApi.Controllers;
using OrderApi.SignalR.Services.Data.ChatEntities;

namespace OrderApi.SignalR.Services.Data
{
    public class Notification
    {
        [Key]
        public int Key { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Message { get; set; }
        public DateTime? Time { get; set; }
       
    }
}
