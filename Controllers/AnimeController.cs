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

        //Get an anime info
        [HttpGet]
        [Route("{getAnimeId:int}")]
        public async Task<ActionResult<List<Anime>>> GetAnime([FromServices] Context context, int getAnimeId)
        {
            var anime = await context.Animes
                                        .Where(a => a.AnimeId == getAnimeId)
                                        .Include(t => t.Tags)
                                        .Include(s => s.Season)
                                        .Include(s => s.Studio)
                                        .Include(c => c.Characters).ThenInclude(s => s.Seiyuu)
                                        .FirstOrDefaultAsync(a => a.AnimeId == getAnimeId);

            if (anime == null) { return NotFound(); }

            return Ok(anime);
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
            // Allow Cors
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            var anime = await context.Animes
                        .Where(a => a.AnimeId == deleteAnimeid)
                        .FirstOrDefaultAsync();
            if (anime != null)
            {
                context.Animes.Remove(anime);
                context.SaveChanges();
                return Ok("Delete success");
            }
            return NotFound();
        }

        //Update anime info
        [HttpPut]
        [Route("{updateAnimeId:int}")]
        public async Task<ActionResult<int>> UpdateAnime([FromServices] Context context, [FromBody] Anime updateAnime, int updateAnimeId)
        {
            var rowsAffected = await _animeService.UpdateAnime(updateAnime, updateAnimeId);
            if (rowsAffected!=0){
                return Ok("Update success");
            }    
            return NotFound();
        }
    }
}