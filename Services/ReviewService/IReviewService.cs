using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IReviewService
    {
        Task<List<Review>> GetReviews(int animeId);
        Task<RatingScoreModel> GetRateScore(int animeId);
        Task<IsClickedModel> IsClicked(IsClickedModel isClickedModel);
    }
}