using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace OrderApi.SignalR
{
    public interface IClientInterface
    {
        Task ClientHook();
    }
    public class ChatHub : Hub
    {
        private readonly ChatRegistry _registry;
        private readonly ShareDb _shareDb;
        private readonly string _id;

        public ChatHub(ChatRegistry registry, ShareDb shareDb)
        {
            _registry = registry;
            _shareDb = shareDb;
            _id = Guid.NewGuid().ToString();
        }

        public void Call()
        {
            Console.WriteLine(_id);
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("User connected: " + Context.ConnectionId);
        }
        public override async Task OnDisconnectedAsync(Exception ecp)
        {
            Console.WriteLine("Disconnected");
        }
        public Task Send(string message)
        {
            return Clients.All.SendAsync("Receive", message);
        }

        public Task SelfPing()
        {
            return Clients.All.SendAsync("Receive","selfPing");
        }
        public void JustAFunction()
        {
            Console.WriteLine("User identifiergg: "+Context.UserIdentifier);
        }
        public Task LeaveRoom(RoomRequest request)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, request.Room);
        }

        public async Task<List<OutputMessage>> JoinRoom(RoomRequest request)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, request.Room);

            return _registry.GetMessages(request.Room)
                .Select(m => m.Output)
                .ToList();
        }
        public Task SendMessage2(InputMessage message)
        {
            var username = Context.User.Claims.FirstOrDefault(x => x.Type == "username").Value;

            var userMessage = new UserMessage(
                new(Context.UserIdentifier, username),
                message.Message,
                message.Room,
                DateTimeOffset.Now
            );
            _registry.AddMessage(message.Room, userMessage);
            return Clients.GroupExcept(message.Room, new[] { Context.ConnectionId })
                .SendAsync("send_message", userMessage.Output);
        }

        public async Task JoinChat(string userName)
        {
            await Clients.All.
                SendAsync("RecieveMessage", "admin", $"{userName} has joined");
        }
        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.chatRoom);
            _shareDb._connection[Context.ConnectionId] = conn;
            
            await Clients.Group(conn.chatRoom).
                SendAsync("JoinSpecificChatRoom", "admin", $"{conn.userName} has joined {conn.chatRoom}");
        }

        public async Task SendMessage(string message)
        {
            if (_shareDb._connection.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.chatRoom).
                    SendAsync("RecieveSpecificMessage",conn.userName,message);
            }
        }
    }

    public record UserConnection(string userName, string chatRoom);

    public class ShareDb
    {
        public readonly ConcurrentDictionary<string,UserConnection> _connection = new();
    }
    public class ChatRegistry
    {
        private readonly Dictionary<string, List<UserMessage>> _roomMessages = new();

        public void CreateRoom(string room)
        {
            _roomMessages[room] = new();
        }

        public void AddMessage(string room, UserMessage message)
        {
            _roomMessages[room].Add(message);
        }

        public List<UserMessage> GetMessages(string room)
        {
            return _roomMessages[room];
        }

        public List<string> GetRooms()
        {
            return _roomMessages.Keys.ToList();
        }
    }

    //public class NotificationRegistry
    //{
    //    public List<string> messages = new() {"Hello from server"};
    //}
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
    public record User(string UserId, string UserName);

    public record RoomRequest(string Room);

    public record InputMessage(
        string Message,
        string Room
    );

    public record OutputMessage(
        string Message,
        string UserName,
        string Room,
        DateTimeOffset SentAt
    );

    public record UserMessage(
        User User,
        string Message,
        string Room,
        DateTimeOffset SentAt
    )
    {
        public OutputMessage Output => new(Message, User.UserName, Room, SentAt);
    }
}
