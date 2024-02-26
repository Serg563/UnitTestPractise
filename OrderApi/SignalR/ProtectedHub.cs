using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace OrderApi.SignalR
{
    //[Authorize(AuthenticationSchemes = Program.CustomCookieScheme)]
    [Authorize(AuthenticationSchemes = Program.CustomCookieScheme + "," + Program.CustomTokenScheme)]
    public class ProtectedHub : Hub
    {
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
