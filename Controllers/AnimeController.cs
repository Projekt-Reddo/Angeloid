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
    [Route("api/anime")]
    public class AnimeController : ControllerBase
    {

        //List all anime in db by some type of group
        // 1: Get anime in this season
        // 2: Get anime in next season
        // 3: Get anime popular all time
        [HttpGet]
        [Route("all/{type:int}")]
        public async Task<ActionResult<List<Anime>>> ListAllAnime([FromServices] Context context, int type)
        {
            switch (type) {
                // 1: Get anime in this season
                case 1:
                    //Get this season time and name in text
                    DateTime thisSeason = DateTime.Today;
                    string thisSeasonName = SeasonNaming.getSeasonInText(thisSeason);

                    //Get 5 anime in this season
                    var thisSeasonAnime = await context.Animes
                                                        .Where(s => s.Season.SeasonName == thisSeasonName) // compare season name
                                                        .Where(s => s.Season.Year == thisSeason.Year.ToString()) // compare season year
                                                        .OrderByDescending(a => a.View) // Get only highest view
                                                        .Take(5) // Get only 5 anime
                                                        .Include(t => t.Tags) // include all tags
                                                        .Include(s => s.Studio) //include studio
                                                        .ToListAsync();

                    if (thisSeasonAnime == null) { break; }

                    return thisSeasonAnime;

                // 2: Get anime in next season
                case 2:
                    //Get next season time and name in text
                    DateTime nextSeason = DateTime.Today.AddMonths(3);
                    string nextSeasonName = SeasonNaming.getSeasonInText(nextSeason);

                    //Get 5 anime in next season
                    var nextSeasonAnime = await context.Animes
                                                        .Where(s => s.Season.SeasonName == nextSeasonName) // compare season name
                                                        .Where(s => s.Season.Year == nextSeason.Year.ToString()) // compare season year
                                                        .OrderByDescending(a => a.View) // Get only highest view
                                                        .Take(5) // Get only 5 anime
                                                        .Include(t => t.Tags.Take(3)) // include all tags
                                                        .Include(s => s.Studio) //include studio
                                                        .ToListAsync();

                    if (nextSeasonAnime == null) { break; }

                    return nextSeasonAnime;

                // 3: Get anime popular all time
                case 3:
                    //Get all time popular anime
                    var allTimePopularAnime = await context.Animes
                                                        .OrderByDescending(a => a.View) // Get highest view anime
                                                        .Take(5) // Get only 5 anime
                                                        .Include(t => t.Tags.Take(3)) // include all tags
                                                        .Include(s => s.Studio) //include studio
                                                        .ToListAsync();

                    if (allTimePopularAnime == null) { break; }

                    return allTimePopularAnime;
            }

            return NotFound();
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