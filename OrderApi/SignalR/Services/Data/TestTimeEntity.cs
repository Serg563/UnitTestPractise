using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.SignalR.Services.Data
{
    public class TestTimeEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime FullTime { get; set; }

        public TimeOnly Time { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan Span { get; set; }

        public DateTimeOffset TimeOffSet { get; set; }
        
    }
}
