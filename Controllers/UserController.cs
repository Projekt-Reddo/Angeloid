using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

//Entity framework
using Microsoft.EntityFrameworkCore;

//DB objects
using Angeloid.DataContext;
using Angeloid.Models;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        // List all users
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<User>>> ListAllUser([FromServices] Context context)
        {
            // Allow cors
            // Response.Headers.Add("Access-Control-Allow-Origin","*");

            var users = await (
                from user in context.Users
                select new User
                {
                    UserId = user.UserId,
                    FacebookId = user.FacebookId,
                    UserName = user.UserName,
                    Password = user.Password,
                    Email = user.Email,
                    Gender = user.Gender,
                    Avatar = user.Avatar,
                    IsAdmin = user.IsAdmin,
                }
            ).ToListAsync();
            if (users == null) { return NotFound(); }
            return users;
        }

        [HttpGet]
        [Route("{userId:int}")]
        public async Task<ActionResult<User>> GetUser([FromServices] Context context, int userId)
        {
            var user = await context.Users
                                .Where(u => u.UserId == userId)
                                .Select(
                                    u => new User
                                    {
                                        UserId = u.UserId,
                                        UserName = u.UserName,
                                        Email = u.Email,
                                        Gender = u.Gender,
                                        Avatar = u.Avatar,
                                        Fullname = u.Fullname,
                                        IsAdmin = u.IsAdmin
                                    }
                                )
                                .FirstOrDefaultAsync();

            if (user != null) { return Ok(user); }
            return NotFound();
        }
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Register([FromServices] Context context, [FromBody] User user)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            // Check existed Username
            var ExistUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (ExistUser != null) { return BadRequest("User name already existed");}
            var ExistEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (ExistEmail != null) { return BadRequest("Email already existed");}

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok("Register Done");
        }
        //Insert new user (register)
        [HttpPost]
        [Route("facebook")]
        public async Task<ActionResult<User>> InsertUser([FromServices] Context context, [FromBody] User user)
        {

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var FacebookUser = await context.Users.Where(u => u.FacebookId == user.FacebookId && u.FacebookId != null).Select(
                u => new User {
                    UserId = u.UserId,
                    IsAdmin = u.IsAdmin,
                    Avatar = u.Avatar
                }
            ).FirstOrDefaultAsync();
            if (FacebookUser != null) { return FacebookUser ;}

            context.Users.Add(user);
            await context.SaveChangesAsync();

            FacebookUser = await context.Users.Where(u => u.FacebookId == user.FacebookId).Select(
                u => new User {
                    UserId = u.UserId,
                    IsAdmin = u.IsAdmin,
                    Avatar = u.Avatar
                }
            ).FirstOrDefaultAsync();
            return FacebookUser;
        }

        //Update user profile
        [HttpPut]
        [Route("profile/{userId:int}")]
        public async Task<ActionResult<User>> UpdateUser([FromServices] Context context, [FromBody] User user, int userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            // Find the email from db, if it exists, return problem
            var existingEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingEmail != null && existingEmail.UserId != user.UserId) {
                throw new Exception("Email Exist");
            }

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.Gender = user.Gender;
                existingUser.Fullname = user.Fullname;
                await context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        //Update user avatar
        [HttpPut]
        [Route("avatar/{userId:int}")]
        public async Task<ActionResult<User>> UpdateAvatar([FromServices] Context context, [FromBody] User user, int userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null)
            {
                existingUser.Avatar = user.Avatar;
                await context.SaveChangesAsync();
                return Ok("Update done.");
            }

            return NotFound();
        }

        //Delete user
        [HttpDelete]
        [Route("{deleteUserid:int}")]
        public async Task<ActionResult<User>> DeleteUser([FromServices] Context context, int deleteUserid)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var existingUser = await context.Users.FirstOrDefaultAsync(user => user.UserId == deleteUserid);
            if (existingUser != null)
            {
                context.Remove(existingUser);
                await context.SaveChangesAsync();
            }
            else { return NotFound(); }
            return Ok("Delete Done");
        }

        //Update user password
        [HttpPut]
        [Route("password/{userId:int}")]
        public async Task<ActionResult<User>> UpdatePassword([FromServices] Context context, [FromBody] UserPassword user, int userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.Password == user.OldPassword)
            {
                existingUser.Password = user.NewPassword;
                await context.SaveChangesAsync();
                return Ok("Update done.");
            }
            throw new Exception("Wrong Password!");
        }
    }
}