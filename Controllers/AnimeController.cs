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
        public AnimeController(IMemoryCache memoryCache)
        {
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
                thisSeasonAnime = await (from anime in context.Animes
                                         where anime.Season.SeasonName == thisSeasonName & anime.Season.Year == thisSeason.Year.ToString()
                                         orderby anime.View descending
                                         select new Anime
                                         {
                                             AnimeId = anime.AnimeId,
                                             AnimeName = anime.AnimeName,
                                             Thumbnail = anime.Thumbnail,
                                             Episode = anime.Episode,
                                             Studio = anime.Studio,
                                             Tags = (from tag in anime.Tags
                                                     orderby tag.TagId ascending
                                                     select new Tag
                                                     {
                                                         TagId = tag.TagId,
                                                         TagName = tag.TagName
                                                     }).Take(3).ToList() //take only 3 tag
                                         }).Take(5).ToListAsync(); //take only 5 anime

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
                nextSeasonAnime = await (from anime in context.Animes
                                         where anime.Season.SeasonName == nextSeasonName & anime.Season.Year == nextSeason.Year.ToString()
                                         orderby anime.View descending
                                         select new Anime
                                         {
                                             AnimeId = anime.AnimeId,
                                             AnimeName = anime.AnimeName,
                                             Thumbnail = anime.Thumbnail,
                                             Episode = anime.Episode,
                                             Studio = anime.Studio,
                                             Tags = (from tag in anime.Tags
                                                     orderby tag.TagId ascending
                                                     select new Tag
                                                     {
                                                         TagId = tag.TagId,
                                                         TagName = tag.TagName
                                                     }).Take(3).ToList()
                                         }).Take(5).ToListAsync();

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
                allTimePopularAnime = await (from anime in context.Animes
                                             orderby anime.View descending
                                             select new Anime
                                             {
                                                 AnimeId = anime.AnimeId,
                                                 AnimeName = anime.AnimeName,
                                                 Thumbnail = anime.Thumbnail,
                                                 Episode = anime.Episode,
                                                 Studio = anime.Studio,
                                                 Tags = (from tag in anime.Tags
                                                         orderby tag.TagId ascending
                                                         select new Tag
                                                         {
                                                             TagId = tag.TagId,
                                                             TagName = tag.TagName
                                                         }).Take(3).ToList()
                                             }).Take(5).ToListAsync();

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