using SimpleChat.Filters;
using SimpleChat.Helpers;
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
    public class ChatMessagesController : ApiController
    {
        private SimpleChatContext db = new SimpleChatContext();

        [HttpGet]
        [Route("chats/{chatId}/chat_messages")]
        public async Task<IHttpActionResult> ListChatMessages(int chatId, int page = 1, int limit = 10)
        {
            var query = (from m in db.ChatMessages select m);
            query = query
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.CreatedAt);

            int totalResults = query.Count();

            query = query
                .Skip((page - 1) * limit)
                .Take(limit);

            List<ChatMessage> chatMessages = query.ToList();
            List<object> respData = new List<object>();

            foreach(ChatMessage chatMessage in chatMessages)
            {
                respData.Add(DataHelper.BuildChatMessageResponseData(chatMessage, chatMessage.User));
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

            return Ok(new { data = respData, meta = respMeta });
        }

        [HttpPost]
        [Route("chats/{chatId}/chat_messages")]
        public async Task<IHttpActionResult> CreateChatMessage(int chatId, [FromBody] ChatRequestData data)
        {
            if (string.IsNullOrEmpty(data.Message))
            {
                return BadRequest();
            }

            try
            {
                object userId;
                Request.Properties.TryGetValue("user_id", out userId);
                int uid = Convert.ToInt32(userId);

                ChatMessage chatMessage = new ChatMessage
                {
                    CreatedAt = DateTime.UtcNow,
                    UserId = uid,
                    ChatId = chatId,
                    Message = data.Message
                };
                db.ChatMessages.Add(chatMessage);
                await db.SaveChangesAsync();

                User user = await db.Users.FindAsync(uid);

                return Ok(new { data = DataHelper.BuildChatMessageResponseData(chatMessage, user), meta = new { } });
            }
            catch(Exception e)
            {
                // log
                return InternalServerError();
            }
            
        }
    }
}
