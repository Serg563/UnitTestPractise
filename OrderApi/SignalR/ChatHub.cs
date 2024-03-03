using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using OrderApi.SignalR.Services;
using OrderApi.SignalR.Services.Data;
using OrderApi.SignalR.Services.Data.ChatEntities;

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
        private readonly ChatService _chatService;

        public ChatHub(ChatRegistry registry, ShareDb shareDb,ChatService chatService)
        {
            _registry = registry;
            _shareDb = shareDb;
            _id = Guid.NewGuid().ToString();
            _chatService = chatService;
        }

        public void Call()
        {
            Console.WriteLine(_id);
        }

        public override async Task OnConnectedAsync()
        {
            
        }

        public async Task CreateGroup(string groupName)
        {
            await _chatService.CreateGroupAsync(groupName,Context.UserIdentifier);
            Console.WriteLine("The group has been created");
        }

        public async Task GetChatMessages(int groupId)
        {
            await base.OnConnectedAsync();
            var messages = await _chatService.GetChatMessages(groupId);

            
            //await Clients.All.SendAsync("ReceiveMessage", messages);
            foreach (var msg in messages)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("ReceiveMessage", msg);
            }
            Console.WriteLine("On user Connected messages");
        }

        public async Task AddUserToGroup(int groupId,string userId)
        {
            var users = await _chatService.GetAllUsersFromChat(groupId);
            await _chatService.AddUserToGroupAsync(groupId, userId);
            foreach (var user in users)
            {
                await Clients.User(user.Id).SendAsync("ReceiveMessage", $"User {userId} has joined room");
            }

            Console.WriteLine("User Joined Chat");
        }

        public async Task SendMessageToGroup(int groupId, string message)
        {
            var users = await _chatService.GetAllUsersFromChat(groupId);
            await _chatService.SendMessageToGroupAsync(groupId, Context.UserIdentifier, message);
            Message newMessage = new Message()
            {
                GroupId = groupId,
                MessageText = message,
                SentDateTime = DateTime.Now,
                UserId = Context.UserIdentifier
            };
            foreach (var user in users)
            {
                await Clients.User(user.Id).SendAsync("ReceiveMessage", newMessage);
            }

            Console.WriteLine("SendMessageToUser");
        }

        public async Task<IEnumerable<Message>> GetUserSpecificGroupMessagesAsync(int groupId)
        {
            return await _chatService.GetUserSpecificGroupAsync(Context.UserIdentifier, groupId);
        }

        public async Task LeaveGroup(int groupId, string userId)
        {
            await _chatService.LeaveGroupOrChatAsync(groupId, userId);
            var users = await _chatService.GetAllUsersFromChat(groupId);
            foreach (var user in users)
            {
                await Clients.User(user.Id).SendAsync("ReceiveMessage", $"User {userId} has leaved the group");
            }

            Console.WriteLine("User leaved group");
        }


        public override async Task OnDisconnectedAsync(Exception ecp)
        {
            Console.WriteLine("Disconnected");
        }
        //public Task Send(string message)
        //{
        //    return Clients.All.SendAsync("Receive", message);
        //}

        //public Task SelfPing()
        //{
        //    return Clients.All.SendAsync("Receive","selfPing");
        //}
        //public void JustAFunction()
        //{
        //    Console.WriteLine("User identifiergg: "+Context.UserIdentifier);
        //}
        //public Task LeaveRoom(RoomRequest request)
        //{
        //    return Groups.RemoveFromGroupAsync(Context.ConnectionId, request.Room);
        //}

        //public async Task<List<OutputMessage>> JoinRoom(RoomRequest request)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, request.Room);

        //    return _registry.GetMessages(request.Room)
        //        .Select(m => m.Output)
        //        .ToList();
        //}
        //public Task SendMessage2(InputMessage message)
        //{
        //    var username = Context.User.Claims.FirstOrDefault(x => x.Type == "username").Value;

        //    var userMessage = new UserMessage(
        //        new(Context.UserIdentifier, username),
        //        message.Message,
        //        message.Room,
        //        DateTimeOffset.Now
        //    );
        //    _registry.AddMessage(message.Room, userMessage);
        //    return Clients.GroupExcept(message.Room, new[] { Context.ConnectionId })
        //        .SendAsync("send_message", userMessage.Output);
        //}

        //public async Task JoinChat(string userName)
        //{
        //    await Clients.All.
        //        SendAsync("RecieveMessage", "admin", $"{userName} has joined");
        //}
        //public async Task JoinSpecificChatRoom(UserConnection conn)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, conn.chatRoom);
        //    _shareDb._connection[Context.ConnectionId] = conn;
            
        //    await Clients.Group(conn.chatRoom).
        //        SendAsync("JoinSpecificChatRoom", "admin", $"{conn.userName} has joined {conn.chatRoom}");
        //}

        //public async Task SendMessage(string message)
        //{
        //    if (_shareDb._connection.TryGetValue(Context.ConnectionId, out UserConnection conn))
        //    {
        //        await Clients.Group(conn.chatRoom).
        //            SendAsync("RecieveSpecificMessage",conn.userName,message);
        //    }
        //}
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
