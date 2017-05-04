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
            DataHelper dataHelper = new DataHelper();
            ChatPaginationResult paginationResult = dataHelper.FindChats(page, limit);

            List<object> respData = new List<object>();
            foreach (Chat c in paginationResult.Results)
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
                    current_page = paginationResult.Page,
                    per_page = paginationResult.Limit,
                    page_count = paginationResult.PageCount,
                    total_count = paginationResult.TotalResults
                }
            };

            return Ok(new { data = respData.ToArray(), meta = respMeta });
        }

        [HttpPost]
        [Route("chats")]
        public async Task<IHttpActionResult> CreateChat([FromBody] ChatRequestData data)
        {
            DataHelper dataHelper = new DataHelper();
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            int uid = Convert.ToInt32(userId);

            if (string.IsNullOrEmpty(data.Name) || string.IsNullOrEmpty(data.Message))
            {
                return BadRequest();
            }

            try
            {
                Chat chat = await dataHelper.CreateChat(data.Name, uid);
                UserChat userChat = await dataHelper.CreateUserChat(chat.ChatId, uid);
                ChatMessage chatMessage = await dataHelper.CreateChatMessage(uid, chat.ChatId, data.Message);

                var respData = new
                {
                    id = chat.ChatId,
                    name = chat.Name,
                    users = dataHelper.FindUsersByChat(chat.ChatId),
                    last_chat_message = dataHelper.FindLastMessageByChat(chat.ChatId)
                };

                return Ok(new { data = respData, meta = new { } });
            }
            catch(Exception)
            {
                // log it
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("chats/{id}")]
        public async Task<IHttpActionResult> UpdateChat(int id, [FromBody] ChatRequestData data)
        {
            DataHelper dataHelper = new DataHelper();
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            int uid = Convert.ToInt32(userId);

            if (string.IsNullOrEmpty(data.Name))
            {
                return BadRequest();
            }

            try
            {
                Chat chat = await dataHelper.UpdateChat(id, uid, data.Name);
                var respData = new
                {
                    id = chat.ChatId,
                    name = chat.Name,
                    users = dataHelper.FindUsersByChat(chat.ChatId),
                    last_chat_message = dataHelper.FindLastMessageByChat(chat.ChatId)
                };

                return Ok(new { data = respData, meta = new { } });
            }
            catch(UnauthorizedAccessException)
            {
                // log it
                return Unauthorized();
            }
            catch(Exception)
            {
                return InternalServerError();
            }
        }
    }
}
