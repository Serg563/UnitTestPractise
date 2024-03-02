using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderApi.Controllers;
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
        public async Task<int> AddMessage(string userId, string message)
        {
            //if (!_usersMessages.ContainsKey(userId))
            //{
            //    _usersMessages[userId] = new List<string>();
            //}
            //_usersMessages[userId].Add(message);
            int generatedKey = 0; 

            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                //if (user.Messages == null)
                //{
                //    user.Messages = new List<Messages>();
                //}
                //user.Messages.Add(new Messages {UserId = userId,Message = message });
                var newMessage = new Data.Notification() { UserId = userId, Message = message, Time = DateTime.Now};
                await _dbContext.Notifications.AddAsync(newMessage);
                await _dbContext.SaveChangesAsync();
                generatedKey = newMessage.Key;
            }
            else
            {
                await _dbContext.ApplicationUsers.AddAsync(new ApplicationUser() { Id = userId });
                var newMessage = new Data.Notification() { UserId = userId, Message = message , Time = DateTime.Now };
                await _dbContext.Notifications.AddAsync(newMessage);
                await _dbContext.SaveChangesAsync();
                generatedKey = newMessage.Key;
            }

            return generatedKey;
        }

        public async Task<List<string>> GetMessages(string userId)
        {
            //List<string> messages;
            //if (_usersMessages.TryGetValue(userId, out messages))
            //{
            //    return messages;
            //}
            var user = _dbContext.Notifications.Where(x => x.UserId == userId);;
            if (user != null)
            {
                return await user.Select(m => m.Message).ToListAsync();
            }
            return new List<string>();

        }
        public async Task<List<Data.Notification>> GetMessagesRefactored(string userId)
        {
            return await _dbContext.Notifications.Where(x => x.UserId == userId).ToListAsync() ;
        }

        public async Task RemoveMessage(string userId,int messageKey)
        {
            var message = await _dbContext.Notifications
                .FirstOrDefaultAsync(m => m.UserId == userId && m.Key == messageKey) ;
            _dbContext.Notifications.Remove(message);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<string>> GetUsers()
        {
            return await _dbContext.ApplicationUsers.Select(u => u.Id).ToListAsync();
        }
    }
}
