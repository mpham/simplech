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
using System.Web.Http.Results;

namespace SimpleChat.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("auth/login")]
        public async Task<IHttpActionResult> Login([FromBody] UserCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest();
            }

            DataHelper dataHelper = new DataHelper();
            User user = dataHelper.FindUserByEmail(credentials.Email);

            if (user == null)
            {
                return NotFound();
            }
            if (user.Password != credentials.Password)
            {
                return Unauthorized();
            }

            var body = new
            {
                data = new
                {
                    id = user.UserId,
                    name = user.Name,
                    email = user.Email
                },
                meta = new { }
            };

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, body);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response.Content.Headers.ContentType.CharSet = "utf-8";
            response.Headers.Add("Authorization", "Bearer " + AuthHelper.CreateToken(user.UserId));

            return new ResponseMessageResult(response);
        }

        [AuthFilter]
        [HttpGet]
        [Route("auth/logout")]
        public void Logout()
        {
            object token;
            if (Request.Properties.TryGetValue("token", out token))
            {
                (new DataHelper()).CreateBlacklistToken(token.ToString());
            }
        }
    }
}
