using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleChat.Helpers
{
    public class DataHelper
    {
        private SimpleChatContext _db = new SimpleChatContext();

        public User FindUserByEmail(string email)
        {
            User user = _db.Users.Where(u => u.Email == email).FirstOrDefault();
            return user;
        }

        public async Task<User> FindUserById(int userId)
        {
            User user = await _db.Users.FindAsync(userId);
            return user;
        }

        public async Task<Chat> CreateChat(string name, int createdBy)
        {
            Chat chat = new Chat();
            chat.Name = name;
            chat.CreatedBy = createdBy;
            _db.Chats.Add(chat);
            await _db.SaveChangesAsync();

            return chat;
        }

        public async Task<Chat> UpdateChat(int chatId, int userId, string name)
        {
            Chat chat = _db.Chats.Find(chatId);

            // only allow original creator to update chat
            if (userId != chat.CreatedBy)
            {
                throw new UnauthorizedAccessException();
            }

            chat.Name = name;

            _db.Entry(chat).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return chat;
        }

        public async Task<UserChat> CreateUserChat(int userId, int chatId)
        {
            List<UserChat> foundUserChat = (from uc in _db.UserChats
                                      where uc.ChatId == chatId && uc.UserId == userId
                                      select uc).ToList();

            if (foundUserChat.Count > 0)
            {
                return foundUserChat[0];
            }

            UserChat userChat = new UserChat
            {
                ChatId = chatId,
                UserId = userId
            };
            _db.UserChats.Add(userChat);
            await _db.SaveChangesAsync();

            return userChat;
        }

        public ChatPaginationResult FindChats(int page, int limit)
        {
            ChatPaginationResult result = new ChatPaginationResult();
            result.Page = page;
            result.Limit = limit;

            var query = (from c in _db.Chats select c);
            query = query
                .OrderBy(e => e.Name)
                .Include("UserChats");

            result.TotalResults = query.Count();

            query = query
                .Skip((page - 1) * limit)
                .Take(limit);

            result.Results = query.ToList();
            return result;
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

        public async Task<BlacklistToken> CreateBlacklistToken(string token)
        {
            BlacklistToken blToken = new BlacklistToken
            {
                Token = token
            };
            _db.BlacklistTokens.Add(blToken);
            await _db.SaveChangesAsync();

            return blToken;
        }

        public BlacklistToken FindBlacklistToken(string token)
        {
            BlacklistToken blt = _db.BlacklistTokens.Where(t => t.Token == token).FirstOrDefault();
            return blt;
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