using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public Task<int> DeleteAnime(int animeId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Anime> GetAnimes(int animeId)
        {
            throw new System.NotImplementedException();
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
            var allAnime = await(from anime in _context.Animes
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
            var allTimePopularAnime = await(from anime in _context.Animes
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
            var nextSeasonAnime = await(from anime in _context.Animes
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
            var thisSeasonAnime = await(from anime in _context.Animes
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

        public Task<int> UpdateAnime(Anime anime, int animeId)
        {
            throw new System.NotImplementedException();
        }

        //Check if an anime is exited or not
        //If exist return it's id else return 0;
        public async Task<int> isExistByAnimeName(Anime anime) {
            var existedAnime = await (from ani in _context.Animes
                                        where ani.AnimeName == anime.AnimeName
                                        select new Anime
                                        {
                                            AnimeId = anime.AnimeId
                                        }).FirstOrDefaultAsync();
            
            if (existedAnime != null) return existedAnime.AnimeId;
            
            return 0;
        }
    }
}