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

            var query = (from c in db.Chats select c);
            query = query
                .OrderBy(e => e.Name)
                .Include("UserChats");

            int totalResults = query.Count();

            query = query
                .Skip((page - 1) * limit)
                .Take(limit);

            List<Chat> chats = query.ToList();

            List<object> respData = new List<object>();
            DataHelper dataHelper = new DataHelper();
            foreach (Chat c in chats)
            {
                var data = new
                {
                    id = c.ChatId,
                    name = c.Name,
                    users = dataHelper.FindUsersByChat(c.ChatId),
                    last_chat_message = dataHelper.FindLastMessageByChat(c.ChatId)
                };

                respData.Add(data);
            }

            var respMeta = new
            {
                pagination = new
                {
                    current_page = page,
                    per_page = limit,
                    page_count = DataHelper.CalculatePageCount(limit, totalResults),
                    total_count = totalResults
                }
            };

            return Ok(new { data = respData.ToArray(), meta = respMeta });
        }

        [HttpPost]
        [Route("chats")]
        public async Task<IHttpActionResult> CreateChat([FromBody] ChatRequestData data)
        {
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            int uid = Convert.ToInt32(userId);

            if (string.IsNullOrEmpty(data.Name) || string.IsNullOrEmpty(data.Message))
            {
                return BadRequest();
            }

            Chat chat = new Chat();
            chat.Name = data.Name;
            chat.CreatedBy = uid;
            db.Chats.Add(chat);
            await db.SaveChangesAsync();

            UserChat userChat = new UserChat
            {
                ChatId = chat.ChatId,
                UserId = uid
            };
            db.UserChats.Add(userChat);
            await db.SaveChangesAsync();

            ChatMessage chatMessage = new ChatMessage
            {
                ChatId = chat.ChatId,
                Message = data.Message,
                UserId = uid,
                CreatedAt = DateTime.UtcNow
            };
            db.ChatMessages.Add(chatMessage);
            await db.SaveChangesAsync();

            DataHelper dataHelper = new DataHelper();
            var respData = new
            {
                id = chat.ChatId,
                name = chat.Name,
                users = dataHelper.FindUsersByChat(chat.ChatId),
                last_chat_message = dataHelper.FindLastMessageByChat(chat.ChatId)
            };

            return Ok(new { data = respData, meta = new { } });
        }

        [HttpPatch]
        [Route("chats/{id}")]
        public async Task<IHttpActionResult> UpdateChat(int id, [FromBody] ChatRequestData data)
        {
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            int uid = Convert.ToInt32(userId);

            if (string.IsNullOrEmpty(data.Name))
            {
                return BadRequest();
            }

            Chat chat = db.Chats.Find(id);

            // only allow original creator to update chat
            if (uid != chat.CreatedBy)
            {
                return Unauthorized();
            }

            chat.Name = data.Name;

            db.Entry(chat).State = EntityState.Modified;
            await db.SaveChangesAsync();

            DataHelper dataHelper = new DataHelper();
            var respData = new
            {
                id = chat.ChatId,
                name = chat.Name,
                users = dataHelper.FindUsersByChat(chat.ChatId),
                last_chat_message = dataHelper.FindLastMessageByChat(chat.ChatId)
            };

            return Ok(new { data = respData, meta = new { } });
        }
    }
}
