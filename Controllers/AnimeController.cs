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
        public async Task<ActionResult<Anime>> UpdateAnime([FromServices] Context context, [FromBody] Anime updateAnime, int updateAnimeId)
        {
            /*
                1.tìm anime trong db với animeId == updateAnimeId
                2.nếu có anime thì update:
                    2.1: seiyuus với characters:
                    2.2: Tags.
                    2.3: Studio
                    2.4: Season.
                    2.5: còn lại dễ :)   
                3.nếu không có thì báo lỗi.
            */
            // mount dbAnime data for changing later :D
            var contextAnime = await context.Animes
                            .Where(a => a.AnimeId == updateAnimeId)
                            .Include(st => st.Studio)
                            .Include(s => s.Season)
                            .FirstOrDefaultAsync();

            //1.tìm anime trong db với animeId == updateAnimeId
            var dbAnime = await (
                                from a in context.Animes
                                where a.AnimeId == updateAnimeId
                                select new Anime
                                {
                                    AnimeId = a.AnimeId,
                                    AnimeName = a.AnimeName,
                                    Content = a.Content,
                                    Thumbnail = a.Thumbnail,
                                    Status = a.Status,
                                    Wallpaper = a.Wallpaper,
                                    Trailer = a.Trailer,
                                    EpisodeDuration = a.EpisodeDuration,
                                    Episode = a.Episode,
                                    StartDay = a.StartDay,
                                    Web = a.Web,
                                    Characters = a.Characters,
                                    StudioId = a.StudioId,
                                    SeasonId = a.SeasonId
                                }
                            ).FirstOrDefaultAsync();
            // Lấy season từ db
            var dbSeason = (from season in context.Seasons
                            where season.SeasonName == updateAnime.Season.SeasonName && season.Year == updateAnime.Season.Year
                            select new Season
                            {
                                SeasonId = season.SeasonId
                            }).FirstOrDefault();

            // 2.nếu có anime thì update:
            if (dbAnime != null)
            {
                // 2.1: seiyuus với characters:

                // 2.1.1 : lấy character từ db
                var dbCharacters = await context.Characters
                                    .Where(ch => ch.AnimeId == updateAnimeId)
                                    .Select(ch => new Character
                                    {
                                        CharacterId = ch.CharacterId,
                                        CharacterName = ch.CharacterName,
                                        AnimeId = updateAnimeId
                                    }).ToListAsync();
                // 2.1.2: lấy characters từ update Anime
                var updateCharactes = (from ch in updateAnime.Characters
                                       select new Character
                                       {
                                           CharacterId = ch.CharacterId,
                                           CharacterName = ch.CharacterName,
                                           CharacterRole = ch.CharacterRole,
                                           CharacterImage = ch.CharacterImage,
                                           Seiyuu = ch.Seiyuu
                                       }).ToList();

                // 2.1.3: remove all chacter's AnimeId foreign key
                context.Characters
                        .Where(ch => ch.AnimeId == updateAnimeId)
                        .ToList()
                        .ForEach(ch => ch.AnimeId = null);
                await context.SaveChangesAsync();

                // 2.1.4: update characters and seiyuu info
                foreach (var character in updateCharactes)
                {
                    // get existed Character by update anime chars id
                    var existChar = await context.Characters
                                .FirstOrDefaultAsync(ch => ch.CharacterId == character.CharacterId);

                    var existSeiyuu = await context.Seiyuus
                                .FirstOrDefaultAsync(se => se.SeiyuuName == character.Seiyuu.SeiyuuName);

                    /// if seiyuu is not exist in db -> insert (new seiyuu)
                    if (existSeiyuu == null)
                    {
                        // add new seiyuu in db
                        context.Seiyuus.Add(
                            new Seiyuu
                            {
                                SeiyuuName = character.Seiyuu.SeiyuuName,
                                SeiyuuImage = character.Seiyuu.SeiyuuImage
                            }
                        );
                        await context.SaveChangesAsync();
                    }
                    else
                    {// update dbseiyuu information
                        existSeiyuu.SeiyuuName = character.Seiyuu.SeiyuuName;
                        existSeiyuu.SeiyuuImage = character.Seiyuu.SeiyuuImage;
                        await context.SaveChangesAsync();
                    }

                    // if character is not exitst in db -> insert (new character)
                    if (existChar == null)
                    {
                        var dbSeiyuu = await (from se in context.Seiyuus
                                              where se.SeiyuuName == character.Seiyuu.SeiyuuName
                                              select new Seiyuu
                                              {
                                                  SeiyuuId = se.SeiyuuId
                                              }).FirstOrDefaultAsync();
                        // add new character in db
                        context.Characters.Add(
                            new Character
                            {
                                CharacterName = character.CharacterName,
                                CharacterRole = character.CharacterRole,
                                CharacterImage = character.CharacterImage,
                                AnimeId = updateAnimeId,
                                SeiyuuId = dbSeiyuu.SeiyuuId
                            }
                        );
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        // if character is existed in db 
                        // get new seiyuu just updated 
                        var dbSeiyuu = await (from se in context.Seiyuus
                                              where se.SeiyuuName == character.Seiyuu.SeiyuuName
                                              select new Seiyuu
                                              {
                                                  SeiyuuId = se.SeiyuuId
                                              }).FirstOrDefaultAsync();
                        // change animeId to updateAnimeId
                        context.Characters
                        .Where(ch => ch.CharacterId == character.CharacterId)
                        .ToList()
                        .ForEach(ch =>
                        {
                            ch.AnimeId = updateAnimeId;
                            ch.CharacterName = character.CharacterName;
                            ch.CharacterImage = character.CharacterImage;
                            ch.CharacterRole = character.CharacterRole;
                            ch.SeiyuuId = dbSeiyuu.SeiyuuId;
                        });
                        await context.SaveChangesAsync();
                    }
                }

                // update tags
                var tags = await context.AnimeTags
                        .Where(at => at.AnimeId == updateAnimeId)
                        .Select(at => new Tag
                        {
                            TagId = at.TagId
                        }).ToListAsync();
                // get updated Tags
                var updateTags = (from at in updateAnime.Tags
                                  select new Tag
                                  {
                                      TagId = at.TagId,
                                      TagName = at.TagName
                                  }).ToList();

                // compare if tags in db and updated Tags is the same
                var rs = Helper.isTheSameTag(tags, updateTags);

                if (rs != true)
                {
                    // load animeTag model
                    var animeTagList = await context.AnimeTags
                                    .Where(at => at.AnimeId == updateAnimeId)
                                    .ToListAsync();
                    // remove all row in AnimeTag db that AnimeId is equal updateAnimeId
                    foreach (var animeTag in animeTagList)
                    {
                        context.AnimeTags.Remove(animeTag);
                    }
                    // save change
                    await context.SaveChangesAsync();

                    // update AnimeTag db with new tags
                    foreach (var updateTag in updateTags)
                    {
                        context.AnimeTags.Add(new AnimeTag
                        {
                            AnimeId = updateAnimeId,
                            TagId = updateTag.TagId
                        });
                    }

                    // Save Change
                    await context.SaveChangesAsync();
                }

                // update studio id
                if (dbAnime.StudioId != updateAnime.StudioId)
                {

                    contextAnime.StudioId = updateAnime.StudioId;
                }
                // update season id
                // if updated season id is equals dbAnime season id
                if (dbAnime.SeasonId != dbSeason.SeasonId)
                {
                    contextAnime.SeasonId = dbSeason.SeasonId;
                }

                // update Anime Name
                if (!dbAnime.AnimeName.Equals(updateAnime.AnimeName))
                {
                    contextAnime.AnimeName = updateAnime.AnimeName;
                }
                // update Anime content
                if (!dbAnime.Content.Equals(updateAnime.Content))
                {
                    contextAnime.Content = updateAnime.Content;
                }
                // update Anime Thumbnail
                if (!dbAnime.Thumbnail.Equals(updateAnime.Thumbnail))
                {
                    contextAnime.Thumbnail = updateAnime.Thumbnail;
                }
                // update Anime status
                if (!dbAnime.Status.Equals(updateAnime.Status))
                {
                    contextAnime.Status = updateAnime.Status;
                }
                // update Anime wallpaper
                if (!dbAnime.Wallpaper.Equals(updateAnime.Wallpaper))
                {
                    contextAnime.Wallpaper = updateAnime.Wallpaper;
                }
                // update Anime Trailer
                if (!dbAnime.Trailer.Equals(updateAnime.Trailer))
                {
                    contextAnime.Trailer = updateAnime.Trailer;
                }
                // update Anime episode duration
                if (!dbAnime.EpisodeDuration.Equals(updateAnime.EpisodeDuration))
                {
                    contextAnime.EpisodeDuration = updateAnime.EpisodeDuration;
                }
                // update Anime episode
                if (!dbAnime.Episode.Equals(updateAnime.Episode))
                {
                    contextAnime.Episode = updateAnime.Episode;
                }
                // update Anime start day
                if (!dbAnime.StartDay.Equals(updateAnime.StartDay))
                {
                    contextAnime.StartDay = updateAnime.StartDay;
                }
                // update Anime Website
                if (!dbAnime.Web.Equals(updateAnime.Web))
                {
                    contextAnime.Web = updateAnime.Web;
                }

                await context.SaveChangesAsync();
                return Ok("Update success");
            }
            return NotFound();
        }
    }
}