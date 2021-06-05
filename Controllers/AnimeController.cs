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
        public AnimeController(IMemoryCache memoryCache) {
            _cache = memoryCache;
        }

        //Get anime in this season: First try to find data in cache 
        //if not call query to db to get data and save to cache
        [HttpGet]
        [Route("thisseason")]
        public async Task<ActionResult<List<Anime>>> ListThisSeasonAnime([FromServices] Context context)
        {
            //Get real time of server
            DateTime thisSeason = DateTime.Today;
            string thisSeasonName = SeasonNaming.getSeasonInText(thisSeason);

            //Declare a return variable
            var thisSeasonAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistThisSeasonAnime = _cache.TryGetValue("CachedThisSeason", out thisSeasonAnime);

            //If not call query to db 
            if (!AlreadyExistThisSeasonAnime)
            {
                //Get 5 anime in this season
                thisSeasonAnime = await context.Animes
                                    .Where(s => s.Season.SeasonName == thisSeasonName) // compare season name
                                    .Where(s => s.Season.Year == thisSeason.Year.ToString()) // compare season year
                                    .OrderByDescending(a => a.View) // Get only highest view
                                    .Take(5) // Get only 5 anime
                                    .Include(t => t.Tags) // include all tags
                                    .Include(s => s.Studio) //include studio
                                    .ToListAsync();

                //Config cache setting
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                    .SetSize(20480)
                                    .SetSlidingExpiration(TimeSpan.FromDays(10));

                //Add value to cache
                _cache.Set("CachedThisSeason", thisSeasonAnime, cacheEntryOptions);
            }

            if (thisSeasonAnime == null) { return NotFound(); }

            return thisSeasonAnime;
        }

        //Get anime in next season: First try to find data in cache 
        //if not call query to db to get data and save to cache
        [HttpGet]
        [Route("nextseason")]
        public async Task<ActionResult<List<Anime>>> ListNextSeasonAnime([FromServices] Context context, int getAnimeId)
        {
            //Get next season time and name in text
            DateTime nextSeason = DateTime.Today.AddMonths(3);
            string nextSeasonName = SeasonNaming.getSeasonInText(nextSeason);

            //Declare a return variable
            var nextSeasonAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistNextSeasonAnime = _cache.TryGetValue("CachedNextSeason", out nextSeasonAnime);

            //If not call query to db 
            if (!AlreadyExistNextSeasonAnime)
            {
                //Get 5 anime in next season
                nextSeasonAnime = await context.Animes
                                        .Where(s => s.Season.SeasonName == nextSeasonName) // compare season name
                                        .Where(s => s.Season.Year == nextSeason.Year.ToString()) // compare season year
                                        .OrderByDescending(a => a.View) // Get only highest view
                                        .Take(5) // Get only 5 anime
                                        .Include(t => t.Tags) // include all tags
                                        .Include(s => s.Studio) //include studio
                                        .ToListAsync();

                //Config cache setting                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                    .SetSize(20480)
                                    .SetSlidingExpiration(TimeSpan.FromDays(10));

                //Add value to cache
                _cache.Set("CachedNextSeason", nextSeasonAnime, cacheEntryOptions);
            }

            if (nextSeasonAnime == null) { return NotFound(); }

            return nextSeasonAnime;
        }

        //Get anime popular all time: First try to find data in cache 
        //if not call query to db to get data and save to cache
        [HttpGet]
        [Route("alltimepopular")]
        public async Task<ActionResult<List<Anime>>> ListAllTimePopularAnime([FromServices] Context context, int getAnimeId)
        {
            //Declare a return variable
            var allTimePopularAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistAllTimePopular = _cache.TryGetValue("CachedAllTimePopular", out allTimePopularAnime);

            //If not call query to db
            if (!AlreadyExistAllTimePopular)
            {
                //Get all time popular anime
                allTimePopularAnime = await context.Animes
                                            .OrderByDescending(a => a.View) // Get highest view anime
                                            .Take(5) // Get only 5 anime
                                            .Include(t => t.Tags) // include all tags
                                            .Include(s => s.Studio) //include studio
                                            .ToListAsync();

                //Config cache setting
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                    .SetSize(20480)
                                    .SetSlidingExpiration(TimeSpan.FromDays(10));

                //Add value to cache
                _cache.Set("CachedAllTimePopular", allTimePopularAnime, cacheEntryOptions);
            }


            if (allTimePopularAnime == null) { return NotFound(); }

            return allTimePopularAnime;
        }

        //Get an anime info
        [HttpGet]
        [Route("{getAnimeId:int}")]
        public async Task<ActionResult<List<Anime>>> GetAnime([FromServices] Context context, int getAnimeId)
        {
            return null;
        }

        //Insert new anime
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Anime>> InsertAnime([FromServices] Context context, [FromBody] Anime inputAnime)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var anime = context.Animes.Where(a => a.AnimeId == inputAnime.AnimeId);


            return Ok(anime);
        }

        //Delete a selected anime
        [HttpDelete]
        [Route("{deleteAnimeid:int}")]
        public async Task<ActionResult<Anime>> DeleteAnime([FromServices] Context context, int deleteAnimeid)
        {

            return Ok("Delete done.");
        }

        //Update anime info
        [HttpPut]
        [Route("{updateAnimeId:int}")]
        public async Task<ActionResult<Anime>> UpdateAnime([FromServices] Context context, [FromBody] Anime updateAnime, int updateAnimeId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var existingAnime = await context.Animes.FirstOrDefaultAsync(a => a.AnimeId == updateAnimeId);

            if (existingAnime != null)
            {
                existingAnime.Wallpaper = updateAnime.Wallpaper;
                existingAnime.Thumbnail = updateAnime.Thumbnail;
                await context.SaveChangesAsync();

                return Ok("Change done.");
            }

            return Ok("Update done.");
        }
    }
}