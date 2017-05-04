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