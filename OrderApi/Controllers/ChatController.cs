using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.SignalR;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OrderApi.SignalR.Services;
using OrderApi.SignalR.Services.Data;
using OrderApi.SignalR.Services.Data.ChatEntities;

namespace OrderApi.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatRegistry _chatRegistry;
        private readonly ChatService _chatService;
        private readonly SignalRContext _context;

        public ChatController(ChatRegistry chatRegistry, ChatService chatService, SignalRContext context)
        {
            _chatRegistry = chatRegistry;
            _chatService = chatService;
            _context = context;
        }

        [HttpGet("GetAllUsersWithDetail")]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersWithDetail()
        {
            return await _context.ApplicationUsers.Include(x => x.UserDetails).ToListAsync();
        }
      
        [HttpGet("GetIndividualChatMessages")]
        public async Task<IEnumerable<Message>> GetIndividualChatMessages(string userId,string user2Id)
        {
            return await _chatService.GetIndividualChatMessages(userId,user2Id);
        }

        [HttpGet("GetChatMessagesByGroupId")]
        public async Task<IEnumerable<Message>> GetChatMessagesByGroupId([FromQuery]int groupId)
        {
            return await _chatService.GetChatMessagesByGroupId(groupId);
        }

        [HttpPost("SendMessageToGroup")]
        public async Task<IActionResult> SendMessageToGroup([FromBody] SendMessageRequest request)
        {
            await _chatService.SendMessageToGroupAsync(request.GroupId, request.SenderId, request.Message);
            return Ok(); 
        }



        [HttpPost("CreateGroupAsync")]
        public async Task<IActionResult> CreateGroupWithMultipleUsers([FromBody] CreateGroupModel createGroup)
        {
            await _chatService.CreateGroupAsync(createGroup.GroupName, createGroup.UserId);
            return Ok();
        }

        [HttpPost("CreateIndividualChat")]
        public async Task<IActionResult> CreateIndividualChat([FromQuery]string userId, [FromQuery]string user2Id)
        {
             await _chatService.CreateIndividualChatAsync(userId, user2Id);
             return Ok();
        }

        [HttpPost("SendMessageToUserAsync")]
        public async Task<IActionResult> SendMessageToUserAsync([FromQuery]int groupId, [FromQuery] string userId, [FromQuery] string message)
        {
            await _chatService.SendMessageToUserAsync(groupId,userId, message);
            return Ok();
        }

        [HttpGet("GetChatMessages")]
        public async Task<ActionResult<IEnumerable<Message>>> GetChatMessages([FromQuery] int groupId)
        {
            var messages = await _chatService.GetChatMessages(groupId);
            return Ok(messages);
        }


        [HttpGet("GetUsersChatsWithMembersAndMessages")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUsersChatsWithMembersAndMessages([FromQuery] string userId)
        {
            var messages = await _chatService.GetUsersChatsWithMembersAndMessages(userId);
            return Ok(messages);
        }

        [HttpGet("GetAllGroups")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllGroups()
        {
            var messages = await _chatService.GetAllGroupsAsync();
            return Ok(messages);
        }






        [AllowAnonymous]
        [HttpGet("/auth")]
        public IActionResult Authenticate(string username)
        {
            var claims = new Claim[]
            {
                new("user_id", Guid.NewGuid().ToString()),
                new("username", username),
            };

            var identity = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync("Cookie", principal);
            return Ok();
        }

        [HttpGet("/create")]
        public IActionResult CreateRoom(string room)
        {
            _chatRegistry.CreateRoom(room);
            return Ok();
        }

        [HttpGet("/list")]
        public IActionResult ListRooms()
        {
            return Ok(_chatRegistry.GetRooms());
        }
    }
    public class SendMessageRequest
    {
        public int GroupId { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
    }

    public record CreateGroupModel(string UserId, string GroupName);
}
