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
    [Route("api/loginout")]
    public class LogInOutController : ControllerBase
    {
        
      //Login
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Login([FromServices] Context context, [FromBody] User user)
        {
            // Allow Cors
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            // Check if model valid
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            var _user = await context.Users
                        .Where(u=> u.UserName==user.UserName && u.Password==user.Password)
                        .Select(
                            u=>new User{
                                UserId=u.UserId,
                                Avatar=u.Avatar,
                                IsAdmin=u.IsAdmin
                            }
                        ).FirstOrDefaultAsync();
            
            if (_user != null)
            {
                return Ok(_user);
            }
            return NotFound();
        }

        //Logout
        [HttpGet]
        [Route("{userid:int}")]
        public async Task<ActionResult<List<Anime>>> Logout([FromServices] Context context, int userId)
        {
            return Ok("Logout success");
        }
    }
}