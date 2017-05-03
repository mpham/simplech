using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SimpleChat.Controllers
{
    public class ChatsController : ApiController
    {
        [HttpGet]
        [Route("chats")]
        public async Task<IHttpActionResult> ListChats()
        {
            return Ok("done");
        }

        [HttpPost]
        [Route("chats")]
        public async Task<IHttpActionResult> CreateChat()
        {
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
