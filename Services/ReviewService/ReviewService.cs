using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;
using Angeloid.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Angeloid.Services
{
    public class ReviewService : IReviewService
    {
        private Context _context;
        private IAnimeService _animeService;
        private IUserService _userService;
        public ReviewService(Context context, IAnimeService animeService, IUserService userService)
        {
            _context = context;
            _animeService = animeService;
            _userService = userService;
        }

        public async Task<RatingScoreModel> GetRateScore(int animeId)
        {
            //Check if anime is exist
            if (await _animeService.isExistByAnimeId(animeId) == null)
            {
                return null;
            }

            //Initial value for return model
            RatingScoreModel rateScore = new RatingScoreModel(){
                one = 0,
                two = 0,
                three = 0,
                four = 0,
                five = 0
            };

            //Get rate list of anime
            var rateList= await _context.Reviews.Where(re => re.AnimeId == animeId)
                                        .Select(re => new Review
                                        {
                                            AnimeId = animeId,
                                            UserId = re.UserId,
                                            RateScore = re.RateScore,
                                        }).ToListAsync();

            //Set rate score to return model 
            rateScore.one = (from rate in rateList where rate.RateScore == 1 select rate).Count();
            rateScore.two = (from rate in rateList where rate.RateScore == 2 select rate).Count();
            rateScore.three = (from rate in rateList where rate.RateScore == 3 select rate).Count();
            rateScore.four = (from rate in rateList where rate.RateScore == 4 select rate).Count();
            rateScore.five = (from rate in rateList where rate.RateScore == 5 select rate).Count();

            return rateScore;
        }

        public async Task<List<Review>> GetReviews(int animeId)
        {
            //Check if anime is exist
            if (await _animeService.isExistByAnimeId(animeId) == null) {
                return null;
            }

            //Get reviews of anime
            var reviews = await _context.Reviews.Where(re => re.AnimeId == animeId && re.Content != null)
                                        .OrderByDescending(re => re.UserId)
                                        .Select(re => new Review {
                                            AnimeId = animeId,
                                            UserId = re.UserId,
                                            User = new User {
                                                Avatar = re.User.Avatar
                                            },
                                            Content = re.Content,
                                        }).ToListAsync();

            //Prevent load null content
            if (reviews.Count >5) {
                reviews.Take(5);
            }

            return reviews;
        }

        public async Task<IsClickedModel> IsClicked(IsClickedModel isClickedModel) {

            //Check if user is exist
            if (await _userService.GetUserById(isClickedModel.UserId) == null) {
                return null;
            }

            //Check if anime is exist
            if (await _animeService.isExistByAnimeId(isClickedModel.AnimeId) == null) {
                return null;
            }

            //Get Review and Rate Score
            var isReview = await _context.Reviews
                            .Where(re => re.AnimeId == isClickedModel.AnimeId && re.UserId == isClickedModel.UserId)
                            .Select(re => new Review {
                                AnimeId = isClickedModel.AnimeId,
                                UserId = isClickedModel.UserId,
                                Content = re.Content,
                                RateScore = re.RateScore,
                            }).FirstOrDefaultAsync();

            //Get Favorite
            var isFavorite = await _context.Favorites
                            .Where(fa => fa.AnimeId == isClickedModel.AnimeId && fa.UserId == isClickedModel.UserId)
                            .FirstOrDefaultAsync();

            //Set status to return model
            isClickedModel.Rated = (isReview != null && isReview.RateScore != 0) ? true : false;
            isClickedModel.Favorite = (isFavorite != null) ? true : false;
            isClickedModel.Reviewed = (isReview.Content != null) ? true : false;

            return isClickedModel;
        }
    }
}