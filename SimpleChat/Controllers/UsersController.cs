using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SimpleChat.Models;
using System.Web.Http.Results;
using SimpleChat.Helpers;
using SimpleChat.Filters;

namespace SimpleChat.Controllers
{
    public class UsersController : ApiController
    {
        private SimpleChatContext db = new SimpleChatContext();

        [AuthFilter]
        [HttpGet]
        [Route("users/current")]
        public async Task<IHttpActionResult> ReadUser()
        {
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            try
            {
                User user = await db.Users.FindAsync(Convert.ToInt32(userId));
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(new { id = user.UserId, name = user.Name, email = user.Email });
            }
            catch (InvalidOperationException)
            {
                // log
            }

            return BadRequest();
        }

        // create user
        [HttpPost]
        [Route("users")]
        public async Task<IHttpActionResult> CreateUser([FromBody] UserCredentials cred)
        {
            if (cred.Password != cred.Password_Confirmation)
            {
                return BadRequest();
            }
            
            // TODO: encrypt password

            User user = new User();
            user.Name = cred.Name;
            user.Email = cred.Email;
            user.Password = cred.Password;

            db.Users.Add(user);
            await db.SaveChangesAsync();

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
        [HttpPatch]
        [Route("users/current")]
        public async Task<IHttpActionResult> UpdateUser([FromBody] UserCredentials cred)
        {
            object userId;
            Request.Properties.TryGetValue("user_id", out userId);
            try
            {
                User user = await db.Users.FindAsync(Convert.ToInt32(userId));
                
                if (user == null)
                {
                    return NotFound();
                }
                
                if (!string.IsNullOrEmpty(cred.Name))
                {
                    user.Name = cred.Name;
                }
                if (!string.IsNullOrEmpty(cred.Email))
                {
                    user.Email = cred.Email;
                }
                if (!string.IsNullOrEmpty(cred.Password))
                {
                    if (cred.Password == cred.Password_Confirmation)
                    {
                        // TODO: encrypt password
                        user.Password = cred.Password;
                    }
                    else
                    {
                        throw new Exception("password error");
                    }
                }
                
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Ok(new { id = user.UserId, name = user.Name, email = user.Email });
            }
            catch (Exception e)
            {
                // log
            }

            return BadRequest();
        }

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserId == id) > 0;
        }
    }
}