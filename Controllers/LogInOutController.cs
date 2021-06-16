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
using Angeloid.Services;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/loginout")]
    public class LogInOutController : ControllerBase
    {
        private readonly ILogInOutService _userService;
        public LogInOutController(ILogInOutService userService)
        {
            _userService = userService;
        }

        //Login
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Login([FromBody] User user)
        {
            // Allow Cors
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            // Check if model valid
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var _user = await _userService.Login(user);

            if (_user != null)
            {
                return Ok(_user);
            }
            return NotFound();
        }

        //Logout
        [HttpGet]
        [Route("{userid:int}")]
        public async Task<ActionResult<User>> Logout([FromServices] Context context, int userId)
        {
            return Ok("Logout success");
        }
    }
}