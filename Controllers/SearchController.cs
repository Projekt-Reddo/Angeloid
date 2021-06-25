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
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<List<Anime>>> SearchAnime(Anime anime)
        {
            ICollection<Anime> searchedAnime = await _searchService.Search(anime);

            return Ok(searchedAnime);
        }
    }
}