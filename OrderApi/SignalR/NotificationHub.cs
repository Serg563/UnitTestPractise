using Microsoft.AspNetCore.SignalR;
using OrderApi.SignalR.Services;

namespace OrderApi.SignalR
{
    public class NotificationHub : Hub
    {
        private readonly ConnectionMapping _connections;
        private readonly NotificationRegistry _notificationRegistry;
        public NotificationHub(NotificationRegistry registry,ConnectionMapping connections)
        {
            this._notificationRegistry = registry;
            this._connections = connections;
        }

        public override async Task OnConnectedAsync()
        {
            #region another way
            //string name = Context.UserIdentifier;

            //_connections.Add(name, Context.ConnectionId);

            //foreach (var userId in _connections.GetAllUsers())
            //{
            //    foreach (var message in _notificationRegistry.messages)
            //    {
            //        //if (userId != Context.UserIdentifier)
            //        //{
            //        //    Clients.User(userId).SendAsync("addChatMessage", message);
            //        //}
            //        Clients.User(userId).SendAsync("addChatMessage", message);
            //    }

            //    Console.WriteLine("there are no messages");
            //}
            //await base.OnConnectedAsync();
            #endregion

            string userId = Context.UserIdentifier;
            await _connections.Add(userId, Context.ConnectionId);

            foreach (var message in await _notificationRegistry.GetMessages(userId))
            {
                //if (userId != Context.UserIdentifier)
                //{
                //    Clients.User(userId).SendAsync("addChatMessage", message);
                //}
                await Clients.User(userId).SendAsync("addChatMessage", message);
            }

            Console.WriteLine("there are no messages");

            //await base.OnConnectedAsync();
        }
        public async Task SendChatMessage(string message)
        {
            string name = Context.UserIdentifier;

            //foreach (var connectionId in _connections.GetConnections(who))
            //{
            //    Clients.All.SendAsync("addChatMessage", name + ": " + message);
            //}

            foreach (var userId in await _connections.GetAllUsers())
            {
                if (userId != Context.UserIdentifier)
                {
                    await _notificationRegistry.AddMessage(userId,message);
                    await Clients.All.SendAsync("addChatMessage", message);
                }
            }
        }
       
        public override async Task OnDisconnectedAsync(Exception e)
        {
            string name = Context.UserIdentifier;

            await _connections.Remove(name, Context.ConnectionId);
            Console.WriteLine("_____________________________USER WAS DISCONNECTED______________________");
        }

        public async Task<IEnumerable<string>> GetAllUsers()
        {
            return await _connections.GetAllUsers();
        }
        public async Task AddUserToGroup()
        {
            Console.WriteLine(Context.UserIdentifier);
        }
    }
}
