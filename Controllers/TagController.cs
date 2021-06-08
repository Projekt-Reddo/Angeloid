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
    [Route("api/tag")]
    public class TagController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Tag>>> ListAllTags([FromServices] Context context)
        {
            var tags = await (
                from tag in context.Tags
                select new Tag
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName
                }
            ).ToListAsync();

            if (tags == null) { return NotFound(); }

            return Ok(tags);
        }
    }
}