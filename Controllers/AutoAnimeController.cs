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
    [Route("api/auto")]
    public class AutoAnimeController : ControllerBase
    {
        private readonly IAutoAnimeService _autoAnimeService;
        public AutoAnimeController(IAutoAnimeService autoAnimeService)
        {
            _autoAnimeService = autoAnimeService;
        }

        [HttpGet]
        [Route("process")]
        public async Task<IActionResult> CheckProcessing() {
            // Check proccessing state of auto anime
            bool isProcessing = AutoAnimeProcessing.getIsProcessing();
            return Ok(isProcessing);
        }

        [HttpGet]
        [Route("add")]
        public async Task<IActionResult> AutoAddAnime() {
            AutoAnimeProcessing.setProcessing();
            await _autoAnimeService.AutoAddAnime();
            AutoAnimeProcessing.setDone();
            return Ok("Done!");
        }
    }
}