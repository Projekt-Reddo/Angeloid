using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//DB objects
using Angeloid.Models;

//Services
using Angeloid.Services;

namespace Angeloid.Controllers
{

    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [Route("{animeId:int}")]
        public async Task<ActionResult<List<Review>>> GetAnimeReviews(int animeId) {
            //Get review list from Service
            var reviews = await _reviewService.GetReviews(animeId);
            
            if (reviews == null) {
                return NotFound();
            }

            return reviews;
        }

        [HttpGet]
        [Route("rate/{animeId:int}")]
        public async Task<ActionResult<RatingScoreModel>> GetRateScore(int animeId)
        {
            //Get rateScores from Service
            var rateScores = await _reviewService.GetRateScore(animeId);

            if (rateScores == null)
            {
                return NotFound();
            }

            return rateScores;
        }

        [HttpPost]
        [Route("check")]
        //Check if User reviewed, favorite, rate
        public async Task<ActionResult<IsClickedModel>> isClicked([FromBody] IsClickedModel isClickedModel) {
            if (isClickedModel.AnimeId == 0 || isClickedModel.UserId == 0)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid) {
                return NotFound();
            }

            var rs = await _reviewService.IsClicked(isClickedModel);

            if (rs == null) {
                return NotFound();
            }

            return rs;
        }

        [HttpPost]
        [Route("")]
        //Add rateScore and review
        public async Task<ActionResult<int>> AddRateScore([FromBody] Review review) {
            if (review.AnimeId == 0 || review.UserId == 0)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var rs = await _reviewService.AddReviewAndRating(review);

            if (rs == 0) {
                return NoContent();
            }

            return rs;
        }
    }
}