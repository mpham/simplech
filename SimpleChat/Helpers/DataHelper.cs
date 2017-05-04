using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleChat.Helpers
{
    public class DataHelper
    {
        private SimpleChatContext _db = new SimpleChatContext();

        public async Task<User> FindUserById(int userId)
        {
            User user = await _db.Users.FindAsync(userId);
            return user;
        }

        public object FindUsersByChat(int chatId)
        {
            var userChats = _db.UserChats
                .Include("User")
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => new { id = uc.User.UserId, name = uc.User.Name, email = uc.User.Email });

            return userChats;
        }

        public object FindLastMessageByChat(int chatId)
        {   
            ChatMessage cm = _db.ChatMessages
                .Include("User")
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            return BuildChatMessageResponseData(cm, cm.User);
        }

        public ChatMessagePaginationResult FindChatMessages(int chatId, int page, int limit)
        {
            ChatMessagePaginationResult result = new ChatMessagePaginationResult();
            result.Page = page;
            result.Limit = limit;

            var query = (from m in _db.ChatMessages select m);
            query = query
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.CreatedAt);

            result.TotalResults = query.Count();

            query = query
                .Skip((page - 1) * limit)
                .Take(limit);

            result.Results = query.ToList();
            return result;
        }

        public async Task<ChatMessage> CreateChatMessage(int userId, int chatId, string message)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                ChatId = chatId,
                Message = message
            };
            _db.ChatMessages.Add(chatMessage);
            await _db.SaveChangesAsync();

            return chatMessage;
        }

        public static int CalculatePageCount(int limit, int total)
        {
            int pageCount = total / limit;
            if ((total % limit) > 0)
            {
                pageCount++;
            }
            return pageCount;
        }

        public static object BuildChatMessageResponseData(ChatMessage chatmessage, User user)
        {
            return new
            {
                id = chatmessage.ChatMessageId,
                chat_id = chatmessage.ChatId,
                user_id = chatmessage.UserId,
                message = chatmessage.Message,
                created_at = chatmessage.CreatedAt,
                user = new
                {
                    id = user.UserId,
                    name = user.Name,
                    email = user.Email
                }
            };
        }
    }
}