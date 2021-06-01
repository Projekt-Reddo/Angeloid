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

        //Insert new user (register)
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Anime>> InsertUser([FromServices] Context context, [FromBody] Anime inputAnime)
        {

            return Ok("Insert done.");
        }

        //Update user profile
        [HttpPut]
        [Route("{userid:int}")]
        public async Task<ActionResult<Anime>> UpdateUser([FromServices] Context context, [FromBody] User user, int userId)
        {

            return Ok("Update done.");
        }
    }
}