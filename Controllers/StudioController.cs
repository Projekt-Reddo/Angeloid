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
    [Route("api/studio")]
    public class StudioController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Studio>>> ListAllStudio([FromServices] Context context)
        {
            var studios = await (
                from studio in context.Studios
                where studio.StudioName != ""
                select new Studio
                {
                    StudioId = studio.StudioId,
                    StudioName = studio.StudioName
                }
            ).ToListAsync();

            if (studios == null) { return NotFound(); }

            return Ok(studios);
        }
    }
}