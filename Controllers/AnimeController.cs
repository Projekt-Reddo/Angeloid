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
            string thisSeasonName = Helper.getSeasonInText(thisSeason);

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
            string nextSeasonName = Helper.getSeasonInText(nextSeason);

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

        //Get anime in the db
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<Anime>>> ListAllAnime([FromServices] Context context, int getAnimeId)
        {
            //Declare a return variable
            var allAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistAll = _cache.TryGetValue("CachedAllAnime", out allAnime);

            //If not call query to db
            if (!AlreadyExistAll)
            {
                //Get all time popular anime
                allAnime = await (from anime in context.Animes
                                            select new Anime
                                            {
                                                AnimeId = anime.AnimeId,
                                                AnimeName = anime.AnimeName,
                                                Status = anime.Status,
                                                View = anime.View,
                                                Thumbnail = anime.Thumbnail,
                                                Episode = anime.Episode,
                                                Studio = anime.Studio,
                                            }).ToListAsync();

                //Config cache setting
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                    .SetSize(40480)
                                    .SetSlidingExpiration(TimeSpan.FromDays(10));

                //Add value to cache
                _cache.Set("CachedAllAnime", allAnime, cacheEntryOptions);
            }


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
        public async Task<ActionResult<Anime>> InsertAnime([FromServices] Context context, [FromBody] Anime inputAnime)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            //Get characters from inputAnime and set inputAnime Characters to null
            var inputCharacters = (from ch in inputAnime.Characters
                                    select new Character
                                    {
                                        CharacterName = ch.CharacterName,
                                        CharacterRole = ch.CharacterRole,
                                        CharacterImage = ch.CharacterImage,
                                        Seiyuu = ch.Seiyuu
                                    }).ToList();
            inputAnime.Characters = null;

            //Get tags from input anime and set inputAnim Tags to null
            var inputTags = (from tg in inputAnime.Tags
                                    select new Tag
                                    {
                                        TagId = tg.TagId
                                    }).ToList();
            inputAnime.Tags = null;

            //Add anime to db
            context.Animes.Add(inputAnime);
            await context.SaveChangesAsync();

            var dbAnime = await (from anime in context.Animes
                                where anime.AnimeName == inputAnime.AnimeName
                                select new Anime {
                                    AnimeId = anime.AnimeId
                                }).FirstOrDefaultAsync();

            // insert characters and seiyuu info
            foreach (var character in inputCharacters)
            {
                //check Seiyuu is existed or not
                var existSeiyuu = await context.Seiyuus
                            .FirstOrDefaultAsync(se => se.SeiyuuName == character.Seiyuu.SeiyuuName);

                // if seiyuu is not exist in db -> insert (new seiyuu)
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

                //Get seiyuuId from inserted seiyuu
                var dbSeiyuu = await (from se in context.Seiyuus
                                        where se.SeiyuuName == character.Seiyuu.SeiyuuName
                                        select new Seiyuu
                                        {
                                            SeiyuuId = se.SeiyuuId
                                        }).FirstOrDefaultAsync();

                // add new character to db
                context.Characters.Add(
                    new Character
                    {
                        CharacterName = character.CharacterName,
                        CharacterRole = character.CharacterRole,
                        CharacterImage = character.CharacterImage,
                        AnimeId = dbAnime.AnimeId,
                        SeiyuuId = dbSeiyuu.SeiyuuId
                    }
                );
                await context.SaveChangesAsync();
            }

            // insert tagId and animeId to maptable AnimeTag
            foreach (var tag in inputTags)
            {
                System.Console.WriteLine(tag.TagId);
                context.AnimeTags.Add(
                    new AnimeTag {
                        AnimeId = dbAnime.AnimeId,
                        TagId = tag.TagId
                    }
                );

                await context.SaveChangesAsync();
            }
            

            return Ok("Add done");
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
                            .Include(st=>st.Studio)
                            .Include(s=>s.Season)
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
                            select new Season {
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
                                           CharacterName = ch.CharacterName,
                                           CharacterRole = ch.CharacterRole,
                                           CharacterImage = ch.CharacterImage,
                                           Seiyuu = ch.Seiyuu
                                       }).ToList();

                // if (!Util.isTheSameChar(dbCharacters, updateCharactes))
                // {

                // 2.1.3: remove all chacter's AnimeId foreign key
                context.Characters
                         .Where(ch => ch.AnimeId == updateAnimeId)
                         .ToList()
                         .ForEach(ch => ch.AnimeId = null);
                await context.SaveChangesAsync();

                // 2.1.4: update characters and seiyuu info
                foreach (var character in updateCharactes)
                {
                    var existChar = await context.Characters
                                .FirstOrDefaultAsync(ch => ch.CharacterName == character.CharacterName);

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
                        .Where(ch => ch.CharacterName == character.CharacterName)
                        .ToList()
                        .ForEach(ch =>
                        {
                            ch.AnimeId = updateAnimeId;
                            ch.SeiyuuId = dbSeiyuu.SeiyuuId;
                        });
                        await context.SaveChangesAsync();
                    }
                }
                // }

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
                if (dbAnime.StudioId != updateAnime.Studio.StudioId)
                {
                    
                    contextAnime.StudioId = updateAnime.Studio.StudioId;
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