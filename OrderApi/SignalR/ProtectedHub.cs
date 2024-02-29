using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using OrderApi.SignalR.Services;

namespace OrderApi.SignalR
{
    //[Authorize(AuthenticationSchemes = Program.CustomCookieScheme)]
    //[Authorize(AuthenticationSchemes = Program.CustomCookieScheme + "," + Program.CustomTokenScheme)]
    public class ProtectedHub : Hub
    {
        private readonly NotificationRegistry _notificationRegistry;
        private string guid;
        public ProtectedHub(NotificationRegistry registry)
        {
            _notificationRegistry = registry;
            guid = Guid.NewGuid().ToString();
            Console.WriteLine("Guid: " + guid);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public string GetGuid()
        {
            return guid;
        }
        public async Task<string> GetAllUsers()
        {
            await _notificationRegistry.GetUsers();
            return guid;
        }

        [Authorize("Token")]
        public object TokenProtected()
        {
            return CompileResult();
        }

        [Authorize("Cookie")]
        public object CookieProtected()
        {
            return CompileResult();
        }

        private object CompileResult() =>
            new
            {
                UserId = Context.UserIdentifier,
                Claims = Context.User.Claims.Select(x => new { x.Type, x.Value })
            };
    }
}
