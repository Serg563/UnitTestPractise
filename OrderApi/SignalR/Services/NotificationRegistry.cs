using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderApi.SignalR.Services.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OrderApi.SignalR.Services
{
    public class NotificationRegistry
    {
        private readonly Dictionary<string, List<string>> _usersMessages = new();
        private readonly SignalRContext _dbContext;
        public NotificationRegistry(SignalRContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddMessage(string userId, string message)
        {
            //if (!_usersMessages.ContainsKey(userId))
            //{
            //    _usersMessages[userId] = new List<string>();
            //}
            //_usersMessages[userId].Add(message);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                //if (user.Messages == null)
                //{
                //    user.Messages = new List<Messages>();
                //}
                //user.Messages.Add(new Messages {UserId = userId,Message = message });
                await _dbContext.Messages.AddAsync(new Messages() { UserId = userId, Message = message });
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                await _dbContext.Users.AddAsync(new Users(){UserId = userId});
                await _dbContext.Messages.AddAsync(new Messages(){UserId = userId,Message = message});
                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task<List<string>> GetMessages(string userId)
        {
            //List<string> messages;
            //if (_usersMessages.TryGetValue(userId, out messages))
            //{
            //    return messages;
            //}
            var user = _dbContext.Messages.Where(x => x.UserId == userId);;
            if (user != null)
            {
                return await user.Select(m => m.Message).ToListAsync();
            }
            return new List<string>();

        }

        public async Task<List<string>> GetUsers()
        {
            return await _dbContext.Users.Select(u => u.UserId).ToListAsync();
        }
    }
}
