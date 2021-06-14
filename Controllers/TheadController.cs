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
    [Route("api/thread/")]
    public class ThreadController : ControllerBase
    {
        [HttpGet]
        [Route("startup")]
        public async Task<ActionResult<List<Thread>>> ListThreadsFirst([FromServices] Context context)
        {
            var threads = await context.Threads
                                            .OrderByDescending(t=>t.ThreadId)
                                            .Take(10)
                                            .ToListAsync();

            if (threads == null) { return NotFound(); }

            return Ok(threads);
        }
        [HttpGet]
        [Route("load/{loadId:int}")]
        public async Task<ActionResult<List<Thread>>> LoadMore([FromServices] Context context, int loadId)
        {
            var threads = await (
                from t in context.Threads
                where t.ThreadId<=loadId-10  && t.ThreadId>loadId-20
                orderby t.ThreadId descending
                select t
            ).ToListAsync();

            if (threads == null) { return NotFound(); }

            return Ok(threads);
        }
    }
}