using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IFavoriteService
    {
        Task<int> AddToFavorite(Favorite favorite);
        Task<List<Anime>> GetFavoriteList(int userId);
        Task<int> DeleteFavorite(Favorite favorite);
    }
}