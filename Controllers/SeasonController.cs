using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//DB objects
using Angeloid.Models;
using Angeloid.Services;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/season")]
    public class SeasonController : ControllerBase
    {
        private readonly ISeasonService _seasonService;
        public SeasonController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Season>>> ListAllSeason()
        {
            var seasons = await _seasonService.ListAllSeasonYear();

            if (seasons == null) { return NotFound(); }

            return Ok(seasons);
        }
    }
}