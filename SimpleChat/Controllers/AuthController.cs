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
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("auth/login")]
        public async Task<IHttpActionResult> Login([FromBody] UserCredentials credentials)
        {
            return Ok("asdf");
        }
    }
}
