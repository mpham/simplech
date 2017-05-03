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

namespace SimpleChat.Controllers
{
    public class UsersController : ApiController
    {
        private SimpleChatContext db = new SimpleChatContext();

        // create user
        [Route("users")]
        public async Task<IHttpActionResult> PostUser([FromBody] UserCredentials cred)
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
                    id = user.Id,
                    name = user.Name,
                    email = user.Email
                },
                meta = new { }
            };

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, body);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response.Content.Headers.ContentType.CharSet = "utf-8";
            response.Headers.Add("Authorization", AuthHelper.CreateToken(user.Id));

            return new ResponseMessageResult(response);
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

            if (id != user.Id)
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

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
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
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}