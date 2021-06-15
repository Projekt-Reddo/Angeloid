using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//DB objects
using Angeloid.Models;

//Services
using Angeloid.Services;

//Cache
using Microsoft.Extensions.Caching.Memory;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly IHomePageService _homePageService;

        public HomeController(IMemoryCache memoryCache, IHomePageService homePageService)
        {
            _cache = memoryCache;
            _homePageService = homePageService;
        }

        //Get anime in this season: First try to find data in cache 
        //if not call query to db to get data and save to cache
        [HttpGet]
        [Route("thisseason")]
        public async Task<ActionResult<List<Anime>>> ListThisSeasonAnime()
        {
            //Declare a return variable
            var thisSeasonAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistThisSeasonAnime = _cache.TryGetValue("CachedThisSeason", out thisSeasonAnime);

            //If not call query to db 
            if (!AlreadyExistThisSeasonAnime)
            {
                //Get this season anime from service
                thisSeasonAnime = await _homePageService.ListThisSeasonAnime();

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
        public async Task<ActionResult<List<Anime>>> ListNextSeasonAnime()
        {
            //Declare a return variable
            var nextSeasonAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistNextSeasonAnime = _cache.TryGetValue("CachedNextSeason", out nextSeasonAnime);

            //If not call query to db 
            if (!AlreadyExistNextSeasonAnime)
            {
                //Get this next anime from service
                nextSeasonAnime = await _homePageService.ListNextSeasonAnime();

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
        public async Task<ActionResult<List<Anime>>> ListAllTimePopularAnime()
        {
            //Declare a return variable
            var allTimePopularAnime = (ActionResult<List<Anime>>)null;

            //Check if data is in Cache
            bool AlreadyExistAllTimePopular = _cache.TryGetValue("CachedAllTimePopular", out allTimePopularAnime);

            //If not call query to db
            if (!AlreadyExistAllTimePopular)
            {
                //Get all time popular anime from service
                allTimePopularAnime = await _homePageService.ListAllTimePopularAnime();

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

    }
}