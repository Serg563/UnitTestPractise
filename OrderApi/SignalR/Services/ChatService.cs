using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OrderApi.SignalR.Services.Data;
using OrderApi.SignalR.Services.Data.ChatEntities;
using System;
using OrderApi.Controllers;

namespace OrderApi.SignalR.Services
{
    public class ChatService
    {
        private readonly SignalRContext _context;
        public ChatService(SignalRContext context)
        {
            _context = context;
        }

        public async Task<int> CreateIndividualChatAsync(string userId, string user2Id)
        {
            //var group = await _context.MessageGroups.FirstOrDefaultAsync(x => x.GroupName == groupName);

            //if (group != null)
            //{
            //    return false;
            //}
            //var existingGroup = await _context.GroupMembers
            //    .Where(gm => (gm.UserId == userId && _context.GroupMembers.Any(gm2 => gm2.UserId == user2Id && gm2.GroupId == gm.GroupId)) ||
            //                 (gm.UserId == user2Id && _context.GroupMembers.Any(gm2 => gm2.UserId == userId && gm2.GroupId == gm.GroupId)))
            //    .Select(gm => gm.GroupId)
            //    .FirstOrDefaultAsync();
            var groupId = await _context.GroupMembers
                .Where(gm => (gm.UserId == userId || gm.UserId == user2Id))
                .GroupBy(gm => gm.GroupId)
                .Where(grp => grp.Count() == 2)
                .Select(grp => grp.Key)
                .FirstOrDefaultAsync();

            if (groupId == 0)
            {
                MessageGroup newGroup = new MessageGroup();
                await _context.MessageGroups.AddAsync(newGroup);
                await _context.SaveChangesAsync();

                var newGroupMember = new GroupMember()
                {
                    GroupId = newGroup.GroupId,
                    UserId = userId,
                    JoinedDateTime = DateTime.Now
                };
                _context.GroupMembers.Add(newGroupMember);
                var newGroupMember2 = new GroupMember()
                {
                    GroupId = newGroup.GroupId,
                    UserId = user2Id,
                    JoinedDateTime = DateTime.Now
                };
                _context.GroupMembers.Add(newGroupMember2);
                await _context.SaveChangesAsync();
                return newGroup.GroupId;
            }
            
            return groupId;
        }

        public async Task CreateGroupAsync(string groupName,string creatorId)
        {
            var existingGroup = await _context.MessageGroups.FirstOrDefaultAsync(m => m.GroupName == groupName);

            if (existingGroup == null)
            {
                MessageGroup newGroup = new MessageGroup(){GroupName = groupName};
                await _context.MessageGroups.AddAsync(newGroup);
                await _context.SaveChangesAsync();
            
                var newGroupMember = new GroupMember()
                {
                    GroupId = newGroup.GroupId,
                    UserId = creatorId,
                    JoinedDateTime = DateTime.Now
                };
                await _context.GroupMembers.AddAsync(newGroupMember);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetChatMessagesByGroupId(int groupId)
        {
            var messages = await _context.Messages.Where(x => x.GroupId == groupId).ToListAsync();
            return messages;
        }

        public async Task SendMessageToGroupAsync(int groupId, string senderId, string message)
        {
            var group = await _context.MessageGroups.FirstOrDefaultAsync(x => x.GroupId == groupId);

            if (group == null)
            {
                return;
            }

            Message newMessage = new Message()
            {
                GroupId = groupId,
                MessageText = message,
                SentDateTime = DateTime.Now,
                UserId = senderId
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

        }
       
        public async Task<List<Message>> GetChatMessages(int groupId)
        {
            var chatMessages = await _context.Messages
                .Where(m => m.GroupId == groupId)
                .ToListAsync();

            return chatMessages;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersFromChat(int groupId)
        {
            var res = await _context.GroupMembers
                .Where(x => x.GroupId == groupId)
                .Include(x => x.User)
                .Select(x => x.User)
                .ToListAsync();
            return res;
        }

        public async Task AddUserToGroupAsync(int groupId, string userId)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                Console.WriteLine("AddUserToGroupAsync is null");
            }

            GroupMember member = new GroupMember()
            {
                UserId = userId,
                GroupId = groupId,
                JoinedDateTime = DateTime.Now
            };
            await _context.GroupMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task LeaveGroupOrChatAsync(int groupId,string userId)
        {
            var groupMember = await _context.GroupMembers
                .Where(x => x.UserId == userId && x.GroupId == groupId)
                .FirstOrDefaultAsync();

            _context.GroupMembers.Remove(groupMember);
            await _context.SaveChangesAsync();

            var memberCount = await _context.GroupMembers
                .Where(x => x.GroupId == groupId)
                .CountAsync();

            if (memberCount == 0)
            {
                var group = await _context.MessageGroups.FirstOrDefaultAsync(x => x.GroupId == groupId);
                _context.MessageGroups.Remove(group);
                _context.SaveChangesAsync();

            }
        }

        public async Task<IEnumerable<MessageGroup>> GetAllGroupsAsync()
        {
            return await _context.MessageGroups.ToListAsync();
        }


        //-------------------------------------------------------------------------------------------



        public async Task<IEnumerable<Message>> GetIndividualChatMessages(string userId, string user2Id)
        {
            var groupIds = await _context.GroupMembers
                .Where(gm => (gm.UserId == userId || gm.UserId == user2Id))
                .GroupBy(gm => gm.GroupId)
                .Where(grp => grp.Count() == 2)
                .Select(grp => grp.Key)
                .FirstOrDefaultAsync();

            var messages = await _context.Messages.Where(x => x.GroupId == groupIds).ToListAsync();
                
            return messages;
        }

      
       
        public async Task<IEnumerable<Message>> GetGroupMessagesAsync(int groupId)
        {
            var messages = await _context.Messages.Where(x => x.GroupId == groupId).ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<MessageGroup>> GetUserGroupsAsync(string userId)
        {
            return await _context.GroupMembers.Where(x => x.UserId == userId)
                .Include(x => x.Group)
                .Select(x => x.Group)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetUserSpecificGroupAsync(string uderId, int groupId)
        {
            var userGroupId = await _context.GroupMembers
                .Where(x => x.UserId == uderId && x.GroupId == groupId)
                .Select(x => x.GroupId)
                .FirstOrDefaultAsync();
            var res = await _context.Messages
                .Where(x => x.GroupId == userGroupId)
                .Include(x => x.Group)
                .ToListAsync();
            return res;
        }

      

        public async Task SendMessageToUserAsync(int groupId,string userId,string message)
        {
            Message newMessage = new Message()
            {
                GroupId = groupId,
                MessageText = message,
                SentDateTime = DateTime.Now,
                UserId = userId
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            //MessageGroup newGroup = new MessageGroup()
            //{
            //    GroupName = currentUserId
            //};
            //await _context.MessageGroups.AddAsync(newGroup);
            //await _context.SaveChangesAsync();

            //GroupMember firstGroupMember = new GroupMember()
            //{
            //    GroupId = newGroup.GroupId,
            //    UserId = currentUserId,
            //    JoinedDateTime = DateTime.Now
            //};
            //GroupMember secondGroupMember = new GroupMember()
            //{
            //    GroupId = newGroup.GroupId,
            //    UserId = userId,
            //    JoinedDateTime = DateTime.Now
            //};
            //await _context.GroupMembers.AddRangeAsync(firstGroupMember,secondGroupMember);
            //await _context.SaveChangesAsync();

            //Message newMessage = new Message()
            //{
            //    GroupId = newGroup.GroupId,
            //    MessageText = message,
            //    SentDateTime = DateTime.Now,
            //};

            //await _context.Messages.AddAsync(newMessage);
            //await _context.SaveChangesAsync();
        }

        public async Task<List<MessageGroup>> GetUsersChatsWithMembersAndMessages(string userId)
        {
            var userGroups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToListAsync();

            var userChats = await _context.MessageGroups
                .Where(c => userGroups.Contains(c.GroupId))
                .Include(x => x.GroupMembers)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.UserDetails)
                .Include(x => x.Messages)
                .ToListAsync();

            return userChats;
        }

    }
}
