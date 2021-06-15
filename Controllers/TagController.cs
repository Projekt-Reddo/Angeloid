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
    [Route("api/tag")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Tag>>> ListAllTags()
        {
            var tags = await _tagService.ListAllTags();

            if (tags == null) { return NotFound(); }

            return Ok(tags);
        }
    }
}