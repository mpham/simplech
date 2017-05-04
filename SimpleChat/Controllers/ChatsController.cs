using SimpleChat.Filters;
using SimpleChat.Helpers;
using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SimpleChat.Controllers
{
    [AuthFilter]
    public class ChatsController : ApiController
    {
        private SimpleChatContext db = new SimpleChatContext();

        [HttpGet]
        [Route("chats")]
        public async Task<IHttpActionResult> ListChats(int page = 1, int limit = 10)
        {
            List<object> respData = new List<object>();
            List<Chat> chats = db.Chats.Include("UserChats").ToList();
            
            foreach(Chat c in chats)
            {
                var data = new
                {
                    id = c.ChatId,
                    name = c.Name,
                    users = DataHelper.FindUsersByChat(c.ChatId),
                    last_chat_message = DataHelper.FindLastMessageByChat(c.ChatId)
                };

                respData.Add(data);
            }

            var respMeta = new { };

            return Ok(new { data = respData.ToArray(), meta = respMeta });
        }

        [HttpPost]
        [Route("chats")]
        public async Task<IHttpActionResult> CreateChat([FromBody] ChatRequestData data)
        {
            Chat chat = new Chat();
            chat.Name = data.Name;
            db.Chats.Add(chat);
            await db.SaveChangesAsync();

            object userId;
            Request.Properties.TryGetValue("user_id", out userId);

            UserChat userChat = new UserChat
            {
                ChatId = chat.ChatId,
                UserId = Convert.ToInt32(userId)
            };
            db.UserChats.Add(userChat);
            await db.SaveChangesAsync();

            ChatMessage chatMessage = new ChatMessage
            {
                ChatId = chat.ChatId,
                Message = data.Message,
                UserId = Convert.ToInt32(userId),
                CreatedAt = DateTime.UtcNow
            };
            db.ChatMessages.Add(chatMessage);
            await db.SaveChangesAsync();

            var respData = new
            {
                id = chat.ChatId,
                name = chat.Name,
                users = DataHelper.FindUsersByChat(chat.ChatId),
                last_chat_message = DataHelper.FindLastMessageByChat(chat.ChatId)
            };

            return Ok(new { data = respData, meta = new { } });
        }

        [HttpPatch]
        [Route("chats/{id}")]
        public async Task<IHttpActionResult> UpdateChat(int id, [FromBody] ChatRequestData data)
        {
            Chat chat = db.Chats.Find(4);
            chat.Name = data.Name;

            db.Entry(chat).State = EntityState.Modified;
            await db.SaveChangesAsync();

            var respData = new
            {
                id = chat.ChatId,
                name = chat.Name,
                users = DataHelper.FindUsersByChat(chat.ChatId),
                last_chat_message = DataHelper.FindLastMessageByChat(chat.ChatId)
            };

            return Ok(new { data = respData, meta = new { } });
        }
    }
}
