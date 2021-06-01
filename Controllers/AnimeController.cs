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

        //List all anime in db
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Anime>>> ListAllAnime([FromServices] Context context)
        {
            var animes = await context.Animes.ToListAsync();

            if (animes == null) { return NotFound(); }
            
            return animes;
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

            return Ok("Insert done.");
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

            return Ok("Update done.");
        }
    }
}