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
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IEmailService _emailService;
        private ITokenService _tokenService;
        public UserController(IUserService userService, IEmailService emailService, ITokenService tokenService)
        {
            _userService = userService;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        // List all users
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<User>>> ListAllUser()
        {
            var users = await _userService.ListAllUser();

            if (users != null) { return users; }
            return NotFound();
        }
        // List Top User for admin dashboard
        [HttpGet]
        [Route("topuser")]
        public async Task<ActionResult<List<User>>> ListTopUser()
        {
            var users = await _userService.ListTopUser();

            if (users != null) { return users; }
            return NotFound();
        }

        [HttpGet]
        [Route("{userId:int}")]
        public async Task<ActionResult<User>> GetUser(int userId)
        {
            var user = await _userService.GetUserById(userId);

            if (user != null) { return Ok(user); }
            return NotFound();
        }
        //Insert new user (register)
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            if (await _userService.Register(user) == 0) { return Conflict(); }
            int rowInserted = await _userService.Register(user);
            return Ok(new { message = "Register Done" });
        }
        [HttpPost]
        [Route("facebook")]
        public async Task<ActionResult<User>> FacebookLogin([FromBody] User user)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return await _userService.FacebookLogin(user);
        }

        //Update user profile
        [HttpPut]
        [Route("profile/{userId:int}")]
        public async Task<ActionResult<User>> UpdateUser([FromBody] User user, int userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (await _userService.IsEmailExist(user))
            {
                throw new Exception("Email Exist");
            }

            if (await _userService.UpdateUserInfo(user, userId) != 0)
            {
                return Ok();
            }

            return NotFound();
        }

        //Update user avatar
        [HttpPut]
        [Route("avatar/{userId:int}")]
        public async Task<ActionResult<User>> UpdateAvatar([FromBody] User user, int userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (await _userService.UpdateUserAvatar(user, userId) != 0)
            {
                return Ok();
            }

            return NotFound();
        }

        //Delete user
        [HttpDelete]
        [Route("{deleteUserid:int}")]
        public async Task<ActionResult<User>> DeleteUser(int deleteUserid)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (await _userService.DeleteUserById(deleteUserid) != 0)
            {
                return Ok();
            }

            return NotFound();
        }

        //Update user password
        [HttpPut]
        [Route("password/{userId:int}")]
        public async Task<ActionResult<User>> UpdatePassword([FromBody] UserPassword user, int userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (await _userService.UpdateUserPassword(user, userId) != 0)
            {
                return Ok();
            }
            throw new Exception("Wrong Password!");
        }

        [HttpPost]
        [Route("forgot")]
        public async Task<ActionResult<User>> ForgotPassword([FromBody] User user)
        {
            var existingUser = await _userService.GetUserByEmail(user.Email);
            //Check whether user Email exist or not
            if (existingUser == null)
            {
                return NotFound();
            }

            //Create token & send email link
            string guid = _tokenService.createToken(existingUser.UserId);
            await _emailService.SendEmailAsync(existingUser.Email, guid);
            return Ok();
        }

        [HttpPost]
        [Route("token")]
        public IActionResult VerifyToken([FromBody] Token token)
        {
            if (_tokenService.getUserIdByToken(token.TokenName) != 0)
            {
                return Ok();
            }
            return NotFound();
        }

        //Reset user password
        [HttpPut]
        [Route("reset")]
        public async Task<ActionResult<User>> ResetPassword([FromBody] UserPassword user)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            int userId = _tokenService.getUserIdByToken(user.Token);

            if (await _userService.ResetUserPassword(user, userId) != 0)
            {
                _tokenService.removeToken(userId);
                return Ok("Update password successfully");
            }

            _tokenService.removeToken(userId);
            return NotFound("Cannot reset Password");
        }

        // Route ro List all Login Time in Current Year
        [HttpGet]
        [Route("LoginTime")]
        public ActionResult<List<string>> ListAllLogin()
        {
            var allLogin = _userService.ReadLoginFromFile();

            return allLogin;
        }
    }
}