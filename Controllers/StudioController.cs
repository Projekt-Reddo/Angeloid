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
    [Route("api/studio")]
    public class StudioController : ControllerBase
    {
        private readonly IStudioService _studioService;
        public StudioController(IStudioService studioService)
        {
            _studioService = studioService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Studio>>> ListAllStudio()
        {
            var studios = await _studioService.ListAllStudio();

            if (studios == null) { return NotFound(); }

            return Ok(studios);
        }
    }
}