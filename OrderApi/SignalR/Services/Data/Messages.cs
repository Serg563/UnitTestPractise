using System.ComponentModel.DataAnnotations;

namespace OrderApi.SignalR.Services.Data
{
    public class Messages
    {
        [Key]
        public int Key { get; set; }
        public string UserId { get; set; }
        public Users User { get; set; }

        public string Message { get; set; }
    }
}
