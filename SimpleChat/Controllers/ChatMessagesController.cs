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
        [HttpGet]
        [Route("chats/{chatId}/chat_messages")]
        public async Task<IHttpActionResult> ListChatMessages(int chatId, int page = 1, int limit = 10)
        {
            DataHelper dataHelper = new DataHelper();
            ChatMessagePaginationResult paginationResult = dataHelper.FindChatMessages(chatId, page, limit);

            List<ChatMessage> chatMessages = paginationResult.Results as List<ChatMessage>;
            List<object> respData = new List<object>();
            foreach (ChatMessage chatMessage in chatMessages)
            {
                respData.Add(DataHelper.BuildChatMessageResponseData(chatMessage, chatMessage.User));
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

            return Ok(new { data = respData, meta = respMeta });
        }

        [HttpPost]
        [Route("chats/{chatId}/chat_messages")]
        public async Task<IHttpActionResult> CreateChatMessage(int chatId, [FromBody] ChatRequestData data)
        {
            DataHelper dataHelper = new DataHelper();
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            int uid = Convert.ToInt32(userId);
            
            if (string.IsNullOrEmpty(data.Message))
            {
                return BadRequest();
            }

            try
            {
                ChatMessage chatMessage = await dataHelper.CreateChatMessage(uid, chatId, data.Message);
                User user = await dataHelper.FindUserById(uid);

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
