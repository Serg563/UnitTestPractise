using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.SignalR;
using System.Security.Claims;
using OrderApi.SignalR.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderApi.Controllers
{
    public class NotificationController : ControllerBase
    {
        private readonly INotificationSink _notificationSink;
        private readonly SignalRContext _context;

        public NotificationController(INotificationSink notificationSink,SignalRContext context)
        {
            _notificationSink = notificationSink;
            _context = context;
        } 

        //[Authorize]
        [HttpGet("/notify")]
        public async Task<IActionResult> Notify(string user, string message)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _notificationSink.PushAsync(new(user, message));
            return Ok();
        }

        [HttpGet("testdb")]
        public async Task<IEnumerable<Users>> testdb()
        {
            return await _context.Users.ToListAsync();
        }

    }
}
