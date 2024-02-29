using System.ComponentModel.DataAnnotations;
using OrderApi.Controllers;

namespace OrderApi.SignalR.Services.Data
{
    public class Messages
    {
        [Key]
        public int Key { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Message { get; set; }

        public DateTime? Time { get; set; }
    }
}
