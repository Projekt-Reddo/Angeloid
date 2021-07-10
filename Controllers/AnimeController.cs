using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//Entity framework
using Microsoft.EntityFrameworkCore;

//DB objects
using Angeloid.DataContext;
using Angeloid.Models;

//Services
using Angeloid.Services;

//Cache
using Microsoft.Extensions.Caching.Memory;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/anime")]
    public class AnimeController : ControllerBase
    {
        //Declare for a Cache 
        private IMemoryCache _cache;
        private readonly IAnimeService _animeService;
        public AnimeController(IMemoryCache memoryCache, IAnimeService animeService)
        {
            _cache = memoryCache;
            _animeService = animeService;
        }

        //Get anime in the db
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<Anime>>> ListAllAnime()
        {
            var allAnime = await _animeService.ListAllAnime();

            if (allAnime == null) { return NotFound(); }

            return allAnime;
        }

        //Get an Anime by Anime Id 
        [HttpGet]
        [Route("{getAnimeId:int}")]
        public async Task<ActionResult<Anime>> GetAnime(int getAnimeId)
        {
            var anime = await _animeService.GetAnime(getAnimeId);
            var viewRs = await _animeService.IncreaseView(anime, anime.AnimeId);
            if (anime == null) { return NotFound(); }

            return anime;
        }

        //Get Anime By List Character Name
        [HttpPost]
        [Route("searchImage")]
        public async Task<ActionResult<List<Anime>>> GetAnimesByCharacterName([FromBody] CharacterName listCharacterName)
        {

            List<Anime> animeList = await _animeService.GetAnimesByCharacterName(listCharacterName);

            if (animeList == null) { return NotFound(); }

            return animeList;
        }

        //Insert new anime
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Anime>> InsertAnime([FromBody] Anime inputAnime)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (await _animeService.isExistByAnimeName(inputAnime) != 0) { return Conflict(); }

            int rowInserted = await _animeService.InsertAnime(inputAnime);

            return Ok(new { message = "Add done" });
        }

        //Delete a selected anime
        [HttpDelete]
        [Route("{deleteAnimeid:int}")]
        public async Task<ActionResult<Anime>> DeleteAnime([FromServices] Context context, int deleteAnimeid)
        {
            var rowsAffected = await _animeService.DeleteAnime(deleteAnimeid);
            if (rowsAffected != 0)
            {
                return Ok(new { message = "Delete Success" });
            }
            return NotFound(new { message = "Delete Fails" });
        }

        //Update anime info
        [HttpPut]
        [Route("{updateAnimeId:int}")]
        public async Task<ActionResult<int>> UpdateAnime([FromServices] Context context, [FromBody] Anime updateAnime, int updateAnimeId)
        {
            var rowsAffected = await _animeService.UpdateAnime(updateAnime, updateAnimeId);
            if (rowsAffected != 0)
            {
                return Ok("Update success");
            }
            return NotFound();
        }
    }
}