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
    [Route("api/season")]
    public class SeasonController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Season>>> ListAllSeason([FromServices] Context context)
        {
            var seasons = await context.Seasons
                                            .OrderBy(x => x.Year)
                                            .Select(x => x.Year)
                                            .Distinct()
                                            .ToListAsync();

            if (seasons == null) { return NotFound(); }

            return Ok(seasons);
        }
    }
}