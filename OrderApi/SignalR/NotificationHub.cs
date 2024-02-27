using Microsoft.AspNetCore.SignalR;
using OrderApi.SignalR.Services;
using OrderApi.SignalR.Services.Data;

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
            Console.WriteLine(Guid.NewGuid().ToString());
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier;
            await _connections.Add(userId, Context.ConnectionId);
            var messages = await _notificationRegistry.GetMessagesRefactored(userId);
            foreach (var msg in messages)
            {
                await Clients.User(userId).SendAsync("addNotification", msg);
            }
        }
        public async Task SendChatMessage(string message)
        {
            foreach (var userId in await _notificationRegistry.GetUsers())
            {
                if (userId != Context.UserIdentifier)
                {
                    var generatedKey = await _notificationRegistry.AddMessage(userId,message);
                    var newMessage = new Messages() {Key = generatedKey, UserId = userId, Message = message };
                    await Clients.User(userId).SendAsync("addNotification", newMessage);
                }
            }
        }

        public async Task RemoveMessage(int messageKey)
        {
            await _notificationRegistry.RemoveMessage(Context.UserIdentifier,messageKey);
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            string name = Context.UserIdentifier;
            await _connections.Remove(name, Context.ConnectionId);
            Console.WriteLine("_____________________________USER WAS DISCONNECTED______________________");
        }

        public async Task<IEnumerable<string>> GetAllUsers()
        {
            return await _notificationRegistry.GetUsers();
        }
        public async Task AddUserToGroup()
        {
            Console.WriteLine(Context.UserIdentifier);
        }

        //public override async Task OnConnectedAsync()
        //{
        //    #region another way
        //    //string name = Context.UserIdentifier;

        //    //_connections.Add(name, Context.ConnectionId);

        //    //foreach (var userId in _connections.GetAllUsers())
        //    //{
        //    //    foreach (var message in _notificationRegistry.messages)
        //    //    {
        //    //        //if (userId != Context.UserIdentifier)
        //    //        //{
        //    //        //    Clients.User(userId).SendAsync("addChatMessage", message);
        //    //        //}
        //    //        Clients.User(userId).SendAsync("addChatMessage", message);
        //    //    }

        //    //    Console.WriteLine("there are no messages");
        //    //}
        //    //await base.OnConnectedAsync();
        //    #endregion

        //    string userId = Context.UserIdentifier;
        //    await _connections.Add(userId, Context.ConnectionId);

        //    foreach (var message in await _notificationRegistry.GetMessagesRefactored(userId))
        //    {
        //        //if (userId != Context.UserIdentifier)
        //        //{
        //        //    Clients.User(userId).SendAsync("addChatMessage", message);
        //        //}
        //        await Clients.User(userId).SendAsync("addChatMessage", message);
        //    }

        //    Console.WriteLine("there are no messages");

        //    //await base.OnConnectedAsync();
        //}
    }
}
