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
            Response.Headers.Add("Access-Control-Allow-Origin","*");

            var users = await context.Users.ToListAsync();
            if (users == null) {return NotFound();}
            return users;
        }

        //Insert new user (register)
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> InsertUser([FromServices] Context context, [FromBody] User user)
        {
            // Allow cors
            Response.Headers.Add("Access-Control-Allow-Origin","*");

            if(!ModelState.IsValid) {return BadRequest(ModelState);}
            // Check existed Username
            var ExistUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (ExistUser != null) { return BadRequest("User name already existed");}

            // var FacebookUser = await context.Users.FirstOrDefaultAsync(u => u.FacebookId == user.FacebookId);
            // if (FacebookUser != null) { return BadRequest("Facebook Login");}

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok("Register done.");
        }

        //Update user profile
        [HttpPut]
        [Route("{userid:int}")]
        public async Task<ActionResult<Anime>> UpdateUser([FromServices] Context context, [FromBody] User user, int userId)
        {

            // Allow cors
            Response.Headers.Add("Access-Control-Allow-Origin","*");
            
            return Ok("Update done.");
        }
    }
}