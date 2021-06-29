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
using Angeloid.Services;

namespace Angeloid.Controllers
{
    [ApiController]
    [Route("api/thread/")]
    public class ThreadController : ControllerBase
    {
        private readonly IThreadService _threadService;
        public ThreadController(IThreadService threadService)
        {
            _threadService = threadService;
        }

        [HttpGet]
        [Route("startup")]
        public async Task<ActionResult<List<Thread>>> ListThreadsFirst()
        {
            var threads = await _threadService.ListThreadFirst();

            if (threads == null) { return NotFound(); }

            return Ok(threads);
        }

        [HttpGet]
        [Route("load/{loadId:int}")]
        public async Task<ActionResult<List<Thread>>> LoadMore(int loadId)
        {
            var threads = await _threadService.LoadMore(loadId);

            if (threads == null) { return NotFound(); }

            return Ok(threads);
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Thread>>> ListAllThread()
        {
            var threads = await _threadService.ListAllThread();

            if (threads == null) { return NotFound(); }

            return threads;
        }

        [HttpDelete]
        [Route("{threadId:int}")]
        public async Task<ActionResult<List<Thread>>> DeleteThread(int threadId)
        {
            var rowDeleted = await _threadService.DeleteThreadById(threadId);

            if (rowDeleted == 0) { return NotFound(); }

            return Ok();
        }
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Thread>> AddnewThread([FromBody] Thread thread)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _threadService.AddNewThread(thread);
            return Ok(new { message = "Add Thread Done" });
        }

        [HttpPost]
        [Route("searchThread")]
        public async Task<ActionResult<List<Thread>>> SearchThread([FromBody] SearchThread searchString)
        {
            var threads = await _threadService.SearchThread(searchString);

            return threads;
        }
    }
}