using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//DB objects
using Angeloid.Models;

//Services
using Angeloid.Services;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/favorite")]
    public class FavoriteController : ControllerBase
    {
        private IFavoriteService _favoriteService;
        private IUserService _userService;
        public FavoriteController(IFavoriteService favoriteService, IUserService userService)
        {
            _favoriteService = favoriteService;
            _userService = userService;
        }

        [HttpGet]
        [Route("{userId:int}")]
        public async Task<ActionResult<List<Anime>>> GetFavoriteList(int userId) {
            //Check if user is exist
            if (await _userService.GetUserById(userId) == null)
            {
                return BadRequest();
            }

            var rs = await _favoriteService.GetFavoriteList(userId);

            return rs;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<int>> AddToFavorite(Favorite favorite) {
            if (favorite.AnimeId == 0 || favorite.UserId == 0)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var rs = await _favoriteService.AddToFavorite(favorite);

            if (rs == 0) {
                return BadRequest();
            }

            if (rs == -1) {
                return Conflict();
            } 

            return rs;
        }

        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<int>> DeleteFavorite([FromBody] Favorite favorite) {
            if (favorite.AnimeId == 0 || favorite.UserId == 0)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var rs = await _favoriteService.DeleteFavorite(favorite);

            if (rs == 0) {
                return BadRequest();
            }

            return rs;
        }
    }
}