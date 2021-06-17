using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

//Models
using Angeloid.Models;
using Angeloid.DataContext;
using Angeloid.Controllers;


namespace Angeloid.Services
{
    public class AnimeService : IAnimeService, IHomePageService
    {
        private Context _context;
        private ISeasonService _seasonService;
        private ICharacterService _characterService;
        private ITagService _tagService;
        public AnimeService(Context context,
                    ISeasonService seasonService,
                    ICharacterService characterService,
                    ITagService tagService
        )
        {
            _context = context;
            _seasonService = seasonService;
            _characterService = characterService;
            _tagService = tagService;
        }

        public async Task<int> DeleteAnime(int animeId)
        {
            var anime = await _context.Animes
                        .Where(a => a.AnimeId == animeId)
                        .FirstOrDefaultAsync();
            if (anime != null)
            {
                _context.Animes.Remove(anime);
                var rowsAffected = _context.SaveChanges();
                return rowsAffected;
            }
            return 0;
        }

        // Get an Anime By Anime Id
        public async Task<Anime> GetAnime(int animeId)
        {
            var anime = await _context.Animes
                                    .Where(anime => anime.AnimeId == animeId)
                                    .Select(
                                        anime => new Anime
                                        {
                                            AnimeId = anime.AnimeId,
                                            AnimeName = anime.AnimeName,
                                            Content = anime.Content,
                                            Thumbnail = anime.Thumbnail,
                                            Status = anime.Status,
                                            Wallpaper = anime.Wallpaper,
                                            Trailer = anime.Trailer,
                                            View = anime.View,
                                            EpisodeDuration = anime.EpisodeDuration,
                                            Episode = anime.Episode,
                                            StartDay = anime.StartDay,
                                            Web = anime.Web,
                                            Characters = (from character in anime.Characters
                                                          select new Character
                                                          {
                                                              CharacterId = character.CharacterId,
                                                              CharacterName = character.CharacterName,
                                                              CharacterRole = character.CharacterRole,
                                                              CharacterImage = character.CharacterImage,
                                                              Seiyuu = character.Seiyuu
                                                          }
                                            ).ToList(),
                                            Season = anime.Season,
                                            SeasonId = anime.SeasonId,
                                            StudioId = anime.StudioId,
                                            Studio = anime.Studio,
                                            Tags = anime.Tags,
                                        }
                                    ).FirstOrDefaultAsync();
            return anime;
        }

        // Get List Anime by Characters Name was detected by AI
        public async Task<List<Anime>> GetAnimesByCharacterName(CharacterName listCharacterName)
        {
            HashSet<string> animeSet = new HashSet<string>();
            
            foreach (string characterName in listCharacterName.listCharacterName)
            {
                var anime = await _context.Characters.
                            Where(c => c.CharacterName == characterName)
                            .Select(
                                c => new Character
                                {
                                    Anime = new Anime
                                    {
                                        AnimeId = c.Anime.AnimeId,
                                        AnimeName = c.Anime.AnimeName,
                                        Thumbnail = c.Anime.Thumbnail
                                    }
                                }
                            ).FirstOrDefaultAsync();
                animeSet.Add(JsonConvert.SerializeObject(anime.Anime));
            }

            List<Anime> animeList = new List<Anime>();
            foreach (string anime in animeSet)
            {
                animeList.Add(JsonConvert.DeserializeObject<Anime>(anime));
            }

            return animeList;
        }

        public async Task<int> InsertAnime(Anime inputAnime)
        {
            var rowInserted = 0;

            //Get characters from inputAnime and set inputAnime Characters to null
            var inputCharacters = await _characterService.getCharacterListFromAnime(inputAnime);
            inputAnime.Characters = null;

            //Get tags from input anime and set inputAnim Tags to null
            var inputTags = await _tagService.getTagListFromAnime(inputAnime);
            inputAnime.Tags = null;

            //Get inputSeason id and insert to FK anime.SeasonId
            var inputSeason = inputAnime.Season;
            inputAnime.Season = null;
            inputAnime.SeasonId = await _seasonService.GetSeasonId(inputSeason);

            //Add anime to db
            _context.Animes.Add(inputAnime);
            rowInserted += await _context.SaveChangesAsync();

            //Get inserted anime id
            var insertedAnimeId = await isExistByAnimeName(inputAnime);

            // insert characters and seiyuu info
            rowInserted += await _characterService.insertListCharacter(inputCharacters, insertedAnimeId);

            // insert tagId and animeId to maptable AnimeTag
            rowInserted += await _tagService.insertAnimeTag(inputTags, insertedAnimeId);

            return rowInserted;
        }

        public async Task<List<Anime>> ListAllAnime()
        {
            //Get all anime
            var allAnime = await (from anime in _context.Animes
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

            return allAnime;
        }

        public async Task<List<Anime>> ListAllTimePopularAnime()
        {
            //Get all time popular anime
            var allTimePopularAnime = await (from anime in _context.Animes
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

            return allTimePopularAnime;
        }

        public async Task<List<Anime>> ListNextSeasonAnime()
        {
            //Get next season time and name in text
            DateTime nextSeason = DateTime.Today.AddMonths(3);
            string nextSeasonName = Helper.getSeasonInText(nextSeason);

            //Get 5 anime in next season
            var nextSeasonAnime = await (from anime in _context.Animes
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

            return nextSeasonAnime;
        }

        public async Task<List<Anime>> ListThisSeasonAnime()
        {
            //Get real time of server
            DateTime thisSeason = DateTime.Today;
            string thisSeasonName = Helper.getSeasonInText(thisSeason);

            //Get 5 anime in this season
            var thisSeasonAnime = await (from anime in _context.Animes
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

            return thisSeasonAnime;
        }

        public async Task<int> UpdateAnime(Anime anime, int animeId)
        {
            var rowsAffected = 0;
            var existAnime = await isExistByAnimeId(animeId);
            /*
                1. get existedAnime in database by animeId
                2. update season by modified anime season.
                3. update characters:
                    3.1: remove all animeId fk of characters in existedAnime
                    3.2: update characters + related seiyuu.
                4. update tags:
                    4.1: remove all animeTags rows in intermediate table (entity) AnimeTags
                    4.2: add new AnimeTags
                5. update others:
                    5.1: name
                    5.2: studio
                    5.3: 
                    ....
                6. SaveChanges.

            */

            // update season by season Id
            existAnime.SeasonId = await _seasonService.GetSeasonId(anime.Season);
            // remove animeid fk
            await _characterService.removeAnimeFK(animeId);

            var updateCharactes = await _characterService.getCharacterListFromAnime(anime);
            //update characters + seiyuu
            rowsAffected += await _characterService.updateCharacter(updateCharactes, animeId);

            /* update animetag*/
            // remove animetags
            rowsAffected += await _tagService.removeAnimeTag(animeId);
            // get updated anime tags
            var tagList = await _tagService.getTagListFromAnime(anime);
            //
            rowsAffected += await _tagService.insertAnimeTag(tagList, animeId);

            // update Anime Name
            if (!existAnime.AnimeName.Equals(anime.AnimeName))
            {
                existAnime.AnimeName = anime.AnimeName;
            }
            // update studio id
            if (existAnime.StudioId != anime.StudioId)
            {
                existAnime.StudioId = anime.StudioId;
            }
            // update Anime content
            if (!existAnime.Content.Equals(anime.Content))
            {
                existAnime.Content = anime.Content;
            }
            // update Anime Thumbnail
            if (!existAnime.Thumbnail.Equals(anime.Thumbnail))
            {
                existAnime.Thumbnail = anime.Thumbnail;
            }
            // update Anime status
            if (!existAnime.Status.Equals(anime.Status))
            {
                existAnime.Status = anime.Status;
            }
            // update Anime wallpaper
            if (!existAnime.Wallpaper.Equals(anime.Wallpaper))
            {
                existAnime.Wallpaper = anime.Wallpaper;
            }
            // update Anime Trailer
            if (!existAnime.Trailer.Equals(anime.Trailer))
            {
                existAnime.Trailer = anime.Trailer;
            }
            // update Anime episode duration
            if (!existAnime.EpisodeDuration.Equals(anime.EpisodeDuration))
            {
                existAnime.EpisodeDuration = anime.EpisodeDuration;
            }
            // update Anime episode
            if (!existAnime.Episode.Equals(anime.Episode))
            {
                existAnime.Episode = anime.Episode;
            }
            // update Anime start day
            if (!existAnime.StartDay.Equals(anime.StartDay))
            {
                existAnime.StartDay = anime.StartDay;
            }
            // update Anime Website
            if (!existAnime.Web.Equals(anime.Web))
            {
                existAnime.Web = anime.Web;
            }
            // _context.Animes.Update(existAnime);
            rowsAffected += await _context.SaveChangesAsync();

            return rowsAffected;
        }

        //Check if an anime is exited or not
        //If exist return it's id else return 0;
        public async Task<int> isExistByAnimeName(Anime anime)
        {
            var existedAnime = await (from ani in _context.Animes
                                      where ani.AnimeName == anime.AnimeName
                                      select new Anime
                                      {
                                          AnimeId = anime.AnimeId
                                      }).FirstOrDefaultAsync();

            if (existedAnime != null) return existedAnime.AnimeId;

            return 0;
        }
        private async Task<Anime> isExistByAnimeId(int animeId)
        {
            var existedAnime = await _context.Animes
                                .FirstOrDefaultAsync(a => a.AnimeId == animeId);

            if (existedAnime != null) return existedAnime;

            return null;
        }
    }
}