using SimpleChat.Filters;
using SimpleChat.Models;
using System;
using System.Collections.Generic;
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
        public async Task<IHttpActionResult> ListChats()
        {
            return Ok("done");
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

            ChatMessage chatMessage = new ChatMessage
            {
                ChatId = chat.ChatId,
                Message = data.Message,
                UserId = Convert.ToInt32(userId),
                CreatedAt = DateTime.UtcNow
            };
            db.ChatMessages.Add(chatMessage);
            await db.SaveChangesAsync();


            
            return Ok("done");
        }

        [HttpPatch]
        [Route("chats/{id}")]
        public async Task<IHttpActionResult> UpdateChat()
        {
            return Ok("done");
        }
    }
}
